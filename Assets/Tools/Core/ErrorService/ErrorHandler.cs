using DG.Tweening;
using Tools.Core.ErrorService.Views;
using UnityEngine;

namespace Tools.Core.ErrorService
{
    public class ErrorHandler : MonoBehaviour
    {
        [SerializeField] private AdErrorView _adErrorAdsViewPrefab;
        
        public void CreateViewWithDelayShowError()
        {
            var faderView = CreateFaderView();
            DOVirtual.DelayedCall(0.5f,() => faderView.ShowError()).Play();
        }

        public AdErrorView CreateFaderView()
        {
            var faderView = Instantiate(_adErrorAdsViewPrefab).GetComponent<AdErrorView>();

            faderView.transform.localPosition = Vector3.zero;
            
            return faderView;
        }
    }
}