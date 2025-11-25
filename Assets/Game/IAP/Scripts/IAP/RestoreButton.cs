using UnityEngine;
using UnityEngine.UI;

namespace IAP
{
    [RequireComponent(typeof(Button))]
    public sealed class RestoreButton : MonoBehaviour
    {
        private void Awake()
        {
            Button button = GetComponent<Button>();

            button.onClick.AddListener(PurchaseManager.RestorePurchase);
        }
    }
}