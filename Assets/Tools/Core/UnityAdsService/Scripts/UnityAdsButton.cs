using System;
using Tools.Core.Container;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.Core.UnityAdsService.Scripts
{
    [RequireComponent(typeof(Button))]
    public class UnityAdsButton : MonoBehaviour
    {
        private UnityAdsService UnityAdsService => CoreContainer.Instance.UnityAdsService;
        private Button WatchButton => GetComponent<Button>();
        
        public event Action OnCanGetReward;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            UnsubscribeAllEvent();
        }

        public void Deactivate()
        {
            WatchButton.interactable = false;
        }
        
        public void Activate()
        {
            WatchButton.interactable = true;
        }

        private void Initialize()
        {
            WatchButton.onClick.AddListener(ShowRewardedAd);
        }
        
        private void ShowRewardedAd()
        {
            var listener = UnityAdsService.ShowRewardedAd();
            
            if (listener != null)
            {
                listener.OnShowCompleteAds += NotifyGetRewarded;
            }
        }

        private void NotifyGetRewarded()
        {
            OnCanGetReward?.Invoke();
        }

        private void UnsubscribeAllEvent()
        {
            OnCanGetReward = null;
        }
    }
}