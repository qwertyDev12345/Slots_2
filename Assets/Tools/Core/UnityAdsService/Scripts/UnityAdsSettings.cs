using UnityEngine;

namespace Tools.Core.UnityAdsService.Scripts
{
    [CreateAssetMenu(menuName = "Core/UnityADS", fileName = "UnityAdsSettings")]
    public class UnityAdsSettings : ScriptableObject
    {
        public string GameIdAndroid;
        public string GameIdIos;
        public bool TestMode;
    }
}