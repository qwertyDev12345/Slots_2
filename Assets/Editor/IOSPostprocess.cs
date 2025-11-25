#if UNITY_IOS
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public static class IOSPostprocess
{
    // --- 1) Точный Podfile: Pods в Unity-iPhone, UnityFramework наследует пути ---
    private const string PodfileContents =
@"source 'https://cdn.cocoapods.org/'
platform :ios, '13.0'

use_frameworks! :linkage => :static
inhibit_all_warnings!

target 'Unity-iPhone' do
  pod 'UnityAds', '~> 4.12.0'
end

target 'UnityFramework' do
  inherit! :search_paths
end
";

    // Порядок: 1) пишем Podfile и чистим дубликаты SDK → 2) фиксим пути/импорты
    [PostProcessBuild(998)]
    public static void GeneratePodfileAndCleanSdk(BuildTarget target, string pathToBuild)
    {
        if (target != BuildTarget.iOS) return;

        // 1) Пишем Podfile
        var podfilePath = Path.Combine(pathToBuild, "Podfile");
        File.WriteAllText(podfilePath, PodfileContents);
        Debug.Log($"[iOS Postprocess] Podfile written → {podfilePath}");

        // 2) Удаляем встроенный Unity Ads SDK, если есть (чтобы не было двух разных источников)
        var adsDir = Path.Combine(pathToBuild, "Libraries/com.unity.ads/Plugins/iOS");
        TryDelete(Path.Combine(adsDir, "UnityAds.framework"));
        TryDelete(Path.Combine(adsDir, "UnityAds.xcframework"));

        // 3) Попробуем пропатчить мосты (импорт угловыми скобками)
        PatchImportToAngleBrackets(Path.Combine(adsDir, "UnityAdsShowListener.h"), "UnityAds/UnityAds.h");
        PatchImportToAngleBrackets(Path.Combine(adsDir, "UnityAdsShowListener.mm"), "UnityAds/UnityAds.h");

        // опционально: локально можно сразу pod install, если выставлен UNITY_RUN_POD_INSTALL=1
        var run = Environment.GetEnvironmentVariable("UNITY_RUN_POD_INSTALL");
        if (run == "1")
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-lc \"cd '{pathToBuild}' && pod install --repo-update\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var p = System.Diagnostics.Process.Start(psi);
            p.WaitForExit();
            Debug.Log($"[iOS Postprocess] pod install exited with code {p.ExitCode}");
        }
    }

    [PostProcessBuild(999)]
    public static void FixSearchPaths(BuildTarget target, string pathToBuild)
    {
        if (target != BuildTarget.iOS) return;

        var projPath = PBXProject.GetPBXProjectPath(pathToBuild);
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);

        var unityApp = proj.GetUnityMainTargetGuid();        // Unity-iPhone
        var unityFw  = proj.GetUnityFrameworkTargetGuid();   // UnityFramework

        // HEADER_SEARCH_PATHS: закрываем все типичные места, куда Pods кладёт UnityAds.h
        string[] headerPaths =
        {
            "$(PODS_ROOT)/Headers/Public",
            "$(PODS_ROOT)/Headers/Public/**",
            "$(PODS_ROOT)/UnityAds/UnityAds.framework/Headers",
            "$(PODS_ROOT)/UnityAds/UnityAds.xcframework/**/Headers",
            "$(PODS_CONFIGURATION_BUILD_DIR)/UnityAds/UnityAds.framework/Headers",
            "$(PODS_CONFIGURATION_BUILD_DIR)/UnityAds/UnityAds.xcframework/**/Headers"
        };

        // FRAMEWORK_SEARCH_PATHS: где линкер найдёт фреймворк
        string[] frameworkPaths =
        {
            "$(PODS_ROOT)",
            "$(PODS_ROOT)/**",
            "$(PODS_CONFIGURATION_BUILD_DIR)"
        };

        foreach (var p in headerPaths)
            proj.AddBuildProperty(unityFw, "HEADER_SEARCH_PATHS", p);
        foreach (var p in frameworkPaths)
            proj.AddBuildProperty(unityFw, "FRAMEWORK_SEARCH_PATHS", p);

        proj.AddBuildProperty(unityApp, "OTHER_LDFLAGS", "-ObjC");
        proj.SetBuildProperty(unityFw,  "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
        proj.SetBuildProperty(unityApp, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

        // --- ADD: In-App Purchase capability for Main target (Unity-iPhone) ---

        // 1) Создадим entitlements-файл, если его нет
        var entitlementsPath = Path.Combine(pathToBuild, "Unity-iPhone.entitlements");
        if (!File.Exists(entitlementsPath))
        {
            var plist =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" " +
                "\"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">" +
                "<plist version=\"1.0\"><dict></dict></plist>";
            File.WriteAllText(entitlementsPath, plist);
            Debug.Log("[iOS Postprocess] Created Unity-iPhone.entitlements");
        }

        // 2) Пропишем entitlements в билд-настройки
        var relEntitlements = "Unity-iPhone.entitlements";
        proj.SetBuildProperty(unityApp, "CODE_SIGN_ENTITLEMENTS", relEntitlements);

        // 3) Добавим capability In-App Purchase
        proj.AddCapability(unityApp, PBXCapabilityType.InAppPurchase, relEntitlements, true);

        // 4) Подстраховочно добавим StoreKit (обычно подтягивается сам)
        proj.AddFrameworkToProject(unityApp, "StoreKit.framework", false);

        // --- END ADD ---

        // --- ADD: NSUserTrackingUsageDescription в Info.plist ---
        try
        {
            var plistPath = Path.Combine(pathToBuild, "Info.plist");
            var plistDoc = new PlistDocument();
            plistDoc.ReadFromFile(plistPath);
            PlistElementDict root = plistDoc.root;

            const string trackingKey = "NSUserTrackingUsageDescription";
            const string trackingText = "We use your device identifier to show relevant ads and improve app experience.";

            // перезапишем/создадим значение
            root.SetString(trackingKey, trackingText);

            plistDoc.WriteToFile(plistPath);
            Debug.Log("[iOS Postprocess] NSUserTrackingUsageDescription added to Info.plist");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[iOS Postprocess] Failed to set NSUserTrackingUsageDescription: {e.Message}");
        }
        // --- END ADD ---

        proj.WriteToFile(projPath);
        Debug.Log("[iOS Postprocess] UnityFramework search paths fixed; -ObjC set for app target; In-App Purchase capability added; NSUserTrackingUsageDescription set.");
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Debug.Log($"[iOS Postprocess] Removed bundled SDK: {path}");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[iOS Postprocess] Failed to remove {path}: {e.Message}");
        }
    }

    private static void PatchImportToAngleBrackets(string filePath, string header)
    {
        try
        {
            if (!File.Exists(filePath)) return;
            var txt = File.ReadAllText(filePath);
            // заменяем "UnityAds/UnityAds.h" => <UnityAds/UnityAds.h>
            txt = txt.Replace($"#import \"{header}\"", $"#import <{header}>");
            txt = txt.Replace($"#include \"{header}\"", $"#include <{header}>");
            File.WriteAllText(filePath, txt);
            Debug.Log($"[iOS Postprocess] Patched import to <{header}> in {Path.GetFileName(filePath)}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[iOS Postprocess] Patch import failed for {filePath}: {e.Message}");
        }
    }
}
#endif