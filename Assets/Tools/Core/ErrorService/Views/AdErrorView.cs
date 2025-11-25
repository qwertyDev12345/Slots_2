using DG.Tweening;
using UnityEngine;

namespace Tools.Core.ErrorService.Views
{
    public class AdErrorView : MonoBehaviour
    {
        [SerializeField] private ImageFader _fader;
        [SerializeField] private Transform _errorText;

        private Tween delayCloseFaderTween;
        
        protected void Start()
        {
            _fader.FadeTo(.5f);
            delayCloseFaderTween =  DOVirtual.DelayedCall(5f, CloseView).Play();
        }

        private void OnDestroy()
        {
            delayCloseFaderTween.Kill();
        }

        public void CloseView()
        {
            _fader.FadeOut(.5f, ()=> Destroy(gameObject));
        }

        public void ShowError()
        {
            _errorText.gameObject.SetActive(true);
            DOVirtual.DelayedCall(1f, () => _errorText.gameObject.SetActive(false)).Play();
            
            _fader.FadeOutAwait(1f, .5f, ()=> Destroy(gameObject));
        }
    }
}