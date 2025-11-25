using System;
using UnityEngine;

namespace Tools.Core.NetworkService
{
    public class NetworkReachabilityService : MonoBehaviour
    {
        public bool IsInternetAvailable { get; private set; }
        
        public event Action<bool> OnChanged;
        
        public void CheckInternet()
        {
            IsInternetAvailable = Application.internetReachability != NetworkReachability.NotReachable;
        }

        private void Update()
        {
            bool newReachability = Application.internetReachability != NetworkReachability.NotReachable;
            if (newReachability != IsInternetAvailable)
            {
                IsInternetAvailable = newReachability;
                OnChanged?.Invoke(IsInternetAvailable);
            }
        }

        public bool TryAddListener(Action<bool> listener)
        {
            if (OnChanged != null)
            {
                foreach (Delegate existingListener in OnChanged.GetInvocationList())
                {
                    if (existingListener == (Delegate)listener)
                    {
                        return false;
                    }
                }
            }

            OnChanged += listener;
            return true;
        }
        
        
    }
}