using System;
using UnityEngine;

namespace Slots.Game.Interface
{
    public class MultiplierAnimation : MonoBehaviour
    {
        public event Action OnEndedAnimation = null;

        public bool IsPlaying => gameObject.activeSelf;
        
        private void Awake() =>
            gameObject.SetActive(false);

        public void ShowAnimation() =>
            gameObject.SetActive(true);

        private void HideAnimation()
        {
            OnEndedAnimation?.Invoke();
            OnEndedAnimation = null;
            
            gameObject.SetActive(false);
        }
    }
}