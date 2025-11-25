using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace IAP
{
    [RequireComponent(typeof(Button))]
    public sealed class PurchaseButton : MonoBehaviour
    {
        [SerializeField] private string _productName = "";
        [SerializeField] private Text _priceText = null;
        
        [SerializeField] private UnityEvent _onSucces = null;
        [SerializeField] private UnityEvent _onFailed = null;

        private ProductInfo _productInfo = null;
        
        private void Awake()
        {
            Button button = GetComponent<Button>();

            button.onClick.AddListener(() =>
            {
                PurchaseManager.Buy(IAPSettings.Data.GetProductInfo(_productName));
            });
        }

        private void OnEnable()
        {
            _productInfo = IAPSettings.Data.GetProductInfo(_productName);
            
            if (_priceText != null)
            {
                _priceText.text = PurchaseManager.GetLocalizedPrice(_productInfo);
            }

            PurchaseManager.OnPurchaseSuccess += TryCallSucces;
            PurchaseManager.OnPurchaseFailed += TryCallFailed;

            if (PurchaseManager.IsProductPurchased(_productInfo))
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            PurchaseManager.OnPurchaseSuccess -= TryCallSucces;
            PurchaseManager.OnPurchaseFailed -= TryCallFailed;
        }

        private void TryCallSucces(ProductInfo productInfo)
        {
            if (_productInfo.Name == productInfo.Name)
            {
                _onSucces?.Invoke();

                if (_productInfo.Type == ProductType.NonConsumable)
                    gameObject.SetActive(false);
            }
        }

        private void TryCallFailed(ProductInfo productInfo, PurchaseFailureReason error)
        {
            if (_productInfo.Name == productInfo.Name)
            {
                _onFailed?.Invoke();
            }
        }
    }
}