using UnityEngine;
using DG.Tweening;
using System.Globalization;
using UnityEngine.UI;

namespace Slots.Game.Interface
{
    [RequireComponent(typeof(Text))]
    public class WinDisplay : MonoBehaviour
    {
        private Text _text = null;

        private Tween _counterTween = null;

        private void Awake()
        {
            _text = GetComponent<Text>();
            _text.text = "0";
        }

        public void SetWin(int count)
        {
            if (_counterTween != null && _counterTween.IsActive())
                _counterTween.Kill();

            var format = new NumberFormatInfo { NumberGroupSeparator = " " };

            _counterTween = DOVirtual.Int(int.Parse(_text.text.Replace(" ", "")), count, 0.35f,
                (value) => _text.text = value.ToString("#,0", format)).Play();
        }
    }
}