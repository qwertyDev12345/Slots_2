using UnityEngine;
using DG.Tweening;
using System.Globalization;
using UnityEngine.UI;
using Slots.Game.Values;
using System;
using Slots.Data.Styles;

namespace Slots.Game.Interface
{
    public class FreeSpinsButton : MonoBehaviour
    {
        public event Action OnPressed = null;

        [SerializeField] private Button _button = null;
        [SerializeField] private Image _icon = null;
        [SerializeField] private Text _text = null;

        private Tween _counterTween = null;

        private StyleSet _pressStyle = null;
        private StyleSet _pauseSet = null;

        private bool _isActive = true;
        
        public bool IsPressed { get; private set; }

        private void Awake()
        {
            FieldGenerator generator = GetComponentInParent<FieldGenerator>();

            _pressStyle = generator.GetStyleSet(StyleSetGroup.Icon_Play);
            _pauseSet = generator.GetStyleSet(StyleSetGroup.Icon_Pause);

            var format = new NumberFormatInfo { NumberGroupSeparator = " " };

            _text.text = FreeSpins.Count.ToString("#,0", format);

            FreeSpins.OnChanged += UpdateSpins;

            _button.onClick.AddListener(() =>
            {
                if (IsPressed)
                {
                    IsPressed = false;

                    _icon.sprite = _pressStyle.GetImageSet().image;
                }
                else
                {
                    if (FreeSpins.Count == 0)
                        return;

                    IsPressed = true;

                    _icon.sprite = _pauseSet.GetImageSet().image;

                    OnPressed?.Invoke();
                }
            });

            UpdateButtonState();
        }

        private void OnDestroy()
        {
            FreeSpins.OnChanged -= UpdateSpins;
        }

        private void UpdateButtonState()
        {
            if (!_isActive)
                return;
            
            _button.interactable = FreeSpins.Count > 0;
        }
        
        private void UpdateSpins(int count)
        {
            if (count == 0 && IsPressed == true)
            {
                IsPressed = false;

                _icon.sprite = _pressStyle.GetImageSet().image;
            }

            if (_counterTween != null && _counterTween.IsActive())
                _counterTween.Kill();

            var format = new NumberFormatInfo { NumberGroupSeparator = " " };

            _counterTween = DOVirtual.Int(int.Parse(_text.text.Replace(" ", "")), count, 0.35f,
                (value) => _text.text = value.ToString("#,0", format)).Play();

            UpdateButtonState();
        }

        public void UnPress()
        {
            IsPressed = false;

            _icon.sprite = _pressStyle.GetImageSet().image;
        }

        public void SetActive(bool active)
        {
            _isActive = active;
            
            _button.interactable = active;
            
            UpdateButtonState();
        }
    }
}