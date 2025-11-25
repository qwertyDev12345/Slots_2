using System;
using Slots.Data.Styles;
using UnityEngine;
using UnityEngine.UI;

namespace Slots.Game.Interface
{
    public class FreeSpinsAnimation : MonoBehaviour
    {
        [SerializeField] private Image _number = null;
        
        private FieldGenerator _fieldGenerator = null;

        public event Action OnEndedAnimation = null;

        public bool IsPlaying => gameObject.activeSelf;
        
        private void Awake()
        {
            _fieldGenerator = GetComponentInParent<FieldGenerator>();
            
            gameObject.SetActive(false);
        }

        public void ShowAnimation(int spinsCount)
        {
            string key = "Number_" + spinsCount;

            StyleSet set = _fieldGenerator.GetStyleSet(key);

            if (set == null)
                throw new NullReferenceException($"Set \"{key}\" not found");

            _number.sprite = set.GetImageSet().image;
            
            gameObject.SetActive(true);
        }

        private void HideAnimation()
        {
            OnEndedAnimation?.Invoke();
            OnEndedAnimation = null;
            
            gameObject.SetActive(false);
        }
    }
}