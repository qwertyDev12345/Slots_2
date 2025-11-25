using System;
using UnityEngine;

namespace IAP
{
    [Serializable]
    public class ProductInfo
    {
        [SerializeField] private string _name;
        [SerializeField] private string _defaultPrice;
        [SerializeField] private ProductType _type;
        [SerializeField] private string _androidId ;
        [SerializeField] private string _iosId;

        public string Name => _name;
        public string DefaultPrice => _defaultPrice;
        public ProductType Type => _type;
        
        public string GetCurrentId()
        {
            #if UNITY_ANDROID
            
                return _androidId;
            
            #elif UNITY_IOS
            
                return _iosId;
            
            #else

                return "";

            #endif
        }
    }
}
