using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IAP
{
    [CreateAssetMenu(fileName = "IAPSettingsData", menuName = "Settings/IAPSettingsData", order = 0)]
    public class IAPSettingsData : ScriptableObject
    {
        [SerializeField] private bool _usDemoIAP = false;
        
        [SerializeField] private List<ProductInfo> _products = new List<ProductInfo>();

        public bool UsDemoIAP => _usDemoIAP || Application.isEditor;

        public ProductInfo GetProductInfo(string name)
        {
            return _products.FirstOrDefault(p => p.Name == name);
        }
        
        public ProductInfo GetProductInfoById(string id)
        {
            return _products.FirstOrDefault(p => p.GetCurrentId() == id);
        }
        
        public IReadOnlyList<ProductInfo> GetAllProductInfos()
        {
            return _products;
        }
    }
}
