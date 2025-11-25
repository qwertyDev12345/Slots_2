using Tools.Core.ErrorService;
using Tools.Core.NetworkService;
using UnityEngine;

namespace Tools.Core.Container
{
    public class CoreContainer : MonoBehaviour
    {
        public NetworkReachabilityService NetworkReachabilityService;
        public ErrorHandler ErrorHandler;
        public UnityAdsService.Scripts.UnityAdsService UnityAdsService;

        public static CoreContainer Instance { get; private set; }
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            CreateSingleton();
        }

        private static void CreateSingleton()
        {
            var containerObject = Resources.Load<CoreContainer>("CoreContainer");

            var instance = Instantiate(containerObject);
            instance.name = "CoreContainer";
            instance.InitializeServices();

            DontDestroyOnLoad(instance);
        }

        private void InitializeServices()
        {
            Instance = this;
        
            UnityAdsService.Initialize();
        }
    }
}