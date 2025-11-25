using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Tools.Core.UnityAdsService.Scripts
{
    public class UnityAdsListener : IUnityAdsLoadListener, IUnityAdsShowListener
    {
        public bool IsAdsLoaded { get; private set; }
        public bool IsAdsStarted { get; private set; }
        
        public event Action OnShowCompleteAds;
        public event Action OnShowFailedAds;

        public void Load(string currentIdShow)
        {
            Advertisement.Load(currentIdShow, this);
        }
        
        public void OnUnityAdsAdLoaded(string placementId)
        {
            IsAdsLoaded = true;
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            OnShowFailedAds?.Invoke();
            Debug.Log($"Failed To Load: [{error}]: {message}");
            OnShowFailedAds = null;
        }
        
        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            if (error == UnityAdsShowError.VIDEO_PLAYER_ERROR)
            {
                OnShowCompleteAds?.Invoke();
                
                Debug.Log($"Show To Failure: [{error}]: {message}");
            }
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            IsAdsStarted = true;
        }

        public void OnUnityAdsShowClick(string placementId)
        {
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                OnShowCompleteAds?.Invoke();
                OnShowCompleteAds = null;
            }
        }
    }
}