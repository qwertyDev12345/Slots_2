using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace IAP
{
    internal sealed class PurchaseManagerBase : IStoreListener
    {
        private IStoreController _storeController;

        private IExtensionProvider _storeExtensionProvider;

        private bool IsInitialized => _storeController != null && _storeExtensionProvider != null;

        public event Action<ProductInfo> OnPurchaseSuccess;

        public event Action<ProductInfo, PurchaseFailureReason> OnPurchaseFaile;

        public bool IsProductPurchased(ProductInfo productInfo)
        {
            return PlayerPrefs.GetString(productInfo.GetCurrentId() + "_purchased", "false") == "true";
        }

        public void InitializePurchasing()
        {
            ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (ProductInfo productInfo in IAPSettings.Data.GetAllProductInfos())
                configurationBuilder.AddProduct(productInfo.GetCurrentId(), (UnityEngine.Purchasing.ProductType)productInfo.Type);

            UnityPurchasing.Initialize(this, configurationBuilder);
        }

        public void BuyProduct(ProductInfo productInfo)
        {
            string productId = productInfo.GetCurrentId();

            if (IsInitialized || IAPSettings.Data.UsDemoIAP)
            {
                if (IAPSettings.Data.UsDemoIAP)
                {
                    if (productInfo.Type == ProductType.NonConsumable)
                    {
                        PlayerPrefs.SetString(productId + "_purchased", "true");
                        PlayerPrefs.Save();
                    }

                    OnPurchaseSuccess?.Invoke(productInfo);

                    Debug.Log(productId + " Buyed!");
                }
                else
                {
                    Product product = _storeController.products.WithID(productId);

                    if (product?.availableToPurchase ?? false)
                    {
                        Debug.Log($"Purchasing product asychronously: '{product.definition.id}'");
                        
                        _storeController.InitiatePurchase(product);
                    }
                    else
                    {
                        Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                        
                        OnPurchaseFailed(product, PurchaseFailureReason.ProductUnavailable);
                    }
                }
            }
            else
            {
                Debug.LogError("IAP not initialized");
            }
        }

        public string GetLocalizedPrice(ProductInfo productInfo)
        {
            string productId = productInfo.GetCurrentId();

            if (IsInitialized && !IAPSettings.Data.UsDemoIAP)
            {
                string localizedPriceString = _storeController.products.WithID(productId).metadata.localizedPriceString;

                PlayerPrefs.SetString(productId + "_last_price", localizedPriceString);
                PlayerPrefs.Save();

                return localizedPriceString;
            }

            return PlayerPrefs.GetString(productId + "_last_price", productInfo.DefaultPrice);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized IAP");

            _storeController = controller;
            _storeExtensionProvider = extensions;

            foreach (ProductInfo productInfo in IAPSettings.Data.GetAllProductInfos())
                GetLocalizedPrice(productInfo);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError("OnInitializeFailed InitializationFailureReason:" + error);
            Debug.LogError(message);
        }

        public void RestorePurchase()
        {
            if (!IsInitialized && !IAPSettings.Data.UsDemoIAP)
                return;

            if (IAPSettings.Data.UsDemoIAP)
                Debug.Log("RestorePurchase RestoreTransactions: true");
            else
            {
                _storeExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(delegate (bool result, string message)
                {
                    if (result)
                        Debug.Log("RestorePurchase RestoreTransactions: " +message + result );
                    else
                        Debug.LogError("RestorePurchase RestoreTransactions:" +message + result);
                });
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            string productId = purchaseEvent.purchasedProduct.definition.id;

            ProductInfo productInfo = IAPSettings.Data.GetProductInfoById(productId);
            
            if (productInfo.Type != ProductType.Consumable)
            {
                PlayerPrefs.SetString(productId + "_purchased", "true");
                PlayerPrefs.Save();
            }

            this.OnPurchaseSuccess?.Invoke(productInfo);

            Debug.Log(productId + " Buyed!");

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            string productId = product.definition.id;

            ProductInfo productInfo = IAPSettings.Data.GetProductInfoById(productId);

            this.OnPurchaseFaile?.Invoke(productInfo, failureReason);

            Debug.Log($"OnPurchaseFailed: FAIL. Product: '{productId}', PurchaseFailureReason: {failureReason}");
        }
    }
}