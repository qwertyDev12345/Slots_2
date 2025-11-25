using UnityEditor;
using UnityEngine;

namespace IAP
{
    [CustomEditor(typeof(IAPSettingsData))]
    internal sealed class IAPSettingsDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal("BOX");
            EditorGUILayout.LabelField("Use 'IAP/Settings' to edit IAP settings");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal("BOX");

            if (GUILayout.Button("Open window"))
            {
                 IAPSettingsWindow.Init();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}