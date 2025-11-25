using System;
using System.Collections;
using System.Collections.Generic;
using Tools.Core.Container;
using Tools.Core.ErrorService;
using Tools.Core.NetworkService;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Tools.Core.UnityAdsService.Scripts
{
    public class UnityAdsService : MonoBehaviour, IUnityAdsInitializationListener
    {
        private const int ADLoadMaxCount = 2;

        private const string RewardedVideoPlacementAndroid = "Rewarded_Android";
        private const string RewardedVideoPlacementIOS = "Rewarded_iOS";

        [SerializeField] private UnityAdsSettings _unityAdsSettings;

        private string currentIdInitialize;

        private NetworkReachabilityService NetworkService => CoreContainer.Instance.NetworkReachabilityService;
        private ErrorHandler ErrorHandler => CoreContainer.Instance.ErrorHandler;
        private string CurrentIdShow { get; set; }
        private Queue<UnityAdsListener> AdsListenersPool { get; set; }
        public bool IsInitialize { get; private set; }
        public bool IsAvailableShow { get; private set; }

        public event Action OnInitialize;
        public event Action<bool> OnAvailableShow;

        public void Initialize()
        {
            InitializePlatform();
            NetworkService.CheckInternet();

            if (NetworkService.IsInternetAvailable)
            {
                InitializeAdvertisement();
            }
            else
            {
                NetworkService.OnChanged += InitializeAdvertisement;
            }

            NetworkService.OnChanged += CheckAvailableInternetConnection;
        }

        public UnityAdsListener ShowRewardedAd()
        {
            if (!NetworkService.IsInternetAvailable || !IsAvailableShow)
            {   
                ErrorHandler.CreateViewWithDelayShowError();
                return null;
            }

            if (AdsListenersPool == null || AdsListenersPool.Count == 0)
            {
                Debug.LogWarning("UnityAds: pool empty, scheduling load & aborting show");
                StartCoroutine(LoadRewardsCoroutine());
                ErrorHandler.CreateViewWithDelayShowError();
                return null;
            }

            var listener = AdsListenersPool.Dequeue();
            StartCoroutine(ShowOnceWhenReady(listener));   // показываем ОДИН раз, когда готово
            StartCoroutine(LoadRewardsCoroutine());        // пополняем пул заранее

            return listener;
        }

        private void CheckAvailableInternetConnection(bool isAvailable)
        {
            if (!isAvailable)
            {
                NetworkService.TryAddListener(AwaitInternetConnection);
                IsAvailableShow = false;
                OnAvailableShow?.Invoke(false);
            }
        }

        private void InitializeAdvertisement(bool isAvailable = true)
        {
            if (!isAvailable) return;

            if (Advertisement.isSupported)
            {
                Debug.Log(Application.platform + " supported by Advertisement");
            }

            Advertisement.Initialize(currentIdInitialize, _unityAdsSettings.TestMode, this);
            NetworkService.OnChanged -= InitializeAdvertisement;
        }

        private void AwaitInternetConnection(bool isAvailable)
        {
            if (AdsListenersPool != null)
            {
                AdsListenersPool.Clear();
            }

            StartCoroutine(LoadRewardsAtNoInternetConnectionCoroutine());
            NetworkService.OnChanged -= AwaitInternetConnection;
        }

        private UnityAdsListener CreateUnityAdsListener()
        {
            var listener = new UnityAdsListener();
            listener.Load(CurrentIdShow);           // внутри listener ставит IsAdsLoaded при готовности
            AdsListenersPool.Enqueue(listener);
            return listener;
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Init Success");
            StartCoroutine(LoadRewardsAtStartCoroutine());
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Init Failed: [{error}]: {message}");
        }

        private IEnumerator LoadRewardsAtStartCoroutine()
        {
            yield return StartCoroutine(LoadRewardsCoroutine());

            Debug.Log("All ADS loaded in pool");
            IsInitialize = true;
            OnInitialize?.Invoke();

            IsAvailableShow = true;
            OnAvailableShow?.Invoke(true);
        }

        private IEnumerator LoadRewardsAtNoInternetConnectionCoroutine()
        {
            yield return StartCoroutine(LoadRewardsCoroutine());

            Debug.Log("All ADS loaded in pool");
            IsAvailableShow = true;
            OnAvailableShow?.Invoke(true);
        }

        private IEnumerator LoadRewardsCoroutine()
        {
            AdsListenersPool ??= new Queue<UnityAdsListener>();

            // было <= (создавало ADLoadMaxCount+1 элементов). Должно быть <
            while (AdsListenersPool.Count < ADLoadMaxCount)
            {
                var listener = CreateUnityAdsListener();
                // ждём загрузку, чтобы потом Show не отдавал NOT_READY
                yield return new WaitUntil(() => listener.IsAdsLoaded);
            }
        }

        /// <summary>
        /// Показывает рекламу ОДИН раз, только когда она реально готова.
        /// Без повторных Advertisement.Show и «ре-траев», чтобы не ловить ложные ошибки.
        /// </summary>
        private IEnumerator ShowOnceWhenReady(UnityAdsListener listener)
        {
            // 1) ждём готовность (safety timeout)
            const float loadTimeout = 6f;
            float t = 0f;
            while (!listener.IsAdsLoaded && t < loadTimeout)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!listener.IsAdsLoaded)
            {
                Debug.LogWarning("UnityAds: timed out waiting for ad to load");
                var f1 = ErrorHandler.CreateFaderView();
                f1.ShowError();
                yield break;
            }

            // 2) готовим UI и подписки
            var faderView = ErrorHandler.CreateFaderView();
            listener.OnShowCompleteAds += faderView.CloseView;
            listener.OnShowFailedAds += faderView.ShowError;

            // 3) один-единственный показ
            Advertisement.Show(CurrentIdShow, listener);

            // 4) ждём старт (без повторных Show)
            const float startTimeout = 4f;
            t = 0f;
            while (!listener.IsAdsStarted && t < startTimeout)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!listener.IsAdsStarted)
            {
                Debug.LogWarning("UnityAds: show did not start in time");
                faderView.ShowError();
                yield break;
            }
        }

        private void InitializePlatform()
        {
#if UNITY_IOS
            currentIdInitialize = _unityAdsSettings.GameIdIos;
            CurrentIdShow       = RewardedVideoPlacementIOS;
#elif UNITY_ANDROID
            currentIdInitialize = _unityAdsSettings.GameIdAndroid;
            CurrentIdShow       = RewardedVideoPlacementAndroid;
#endif
        }
    }
}