using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace IAP
{
    [RequireComponent(typeof(Animator))]
    public sealed class PurchaseManager : MonoBehaviour
    {
        private static PurchaseManager _current = null;
        private static PurchaseManagerBase _purchaseManager = null;
        
        public static event Action<ProductInfo> OnPurchaseSuccess = null;
        public static event Action<ProductInfo, PurchaseFailureReason> OnPurchaseFailed = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            string objectName = "[PurchaseManager]";
            
            PurchaseManager purchaseManager = Resources.Load<PurchaseManager>(objectName);
            
            _current = Instantiate(purchaseManager);

            DontDestroyOnLoad(_current);

            _current.name = objectName;
            
            _current._animator.enabled = false;
            _current._fakePurchasePanel.SetActive(false);

            _purchaseManager = new PurchaseManagerBase();
            _purchaseManager.InitializePurchasing();

            _purchaseManager.OnPurchaseSuccess += (product) => OnPurchaseSuccess?.Invoke(product);
            _purchaseManager.OnPurchaseFaile += (product, error) => OnPurchaseFailed?.Invoke(product, error);
        }

        public static bool IsProductPurchased(ProductInfo product)
        {
            return _purchaseManager.IsProductPurchased(product);
        }

        public static string GetLocalizedPrice(ProductInfo product)
        {
            return _purchaseManager.GetLocalizedPrice(product);
        }

        public static void Buy(ProductInfo product)
        {
            if (IAPSettings.Data.UsDemoIAP)
                _current.TryFakePurchase(product);
            else
                _purchaseManager.BuyProduct(product);
        }

        public static void RestorePurchase()
        { 
            _purchaseManager.RestorePurchase();
        }

        [SerializeField] private Animator _animator = null;
        [SerializeField] private Text _productNameText = null;
        [SerializeField] private GameObject _fakePurchasePanel = null;

        private ProductInfo _productInfo = null;

        private void TryFakePurchase(ProductInfo product)
        {
            _animator.enabled = true;
                
            _fakePurchasePanel.SetActive(true);
            
            _productInfo = product;

            _productNameText.text = $"Product: " + _productInfo.Name;
        }

        private void DisablePanel()
        {
            _fakePurchasePanel.SetActive(false);
            
            _animator.enabled = false;
            
            _animator.SetTrigger("Reset");
        }

        public void Purchase()
        {
            _purchaseManager.BuyProduct(_productInfo);
                    
            _productInfo = null;
            
            _animator.SetTrigger("Hide");
        }

        public void Cancel()
        {
            OnPurchaseFailed?.Invoke(
                _productInfo, 
                PurchaseFailureReason.UserCancelled);
                    
            _productInfo = null;
            
            _animator.SetTrigger("Hide");
        }
    }
}
