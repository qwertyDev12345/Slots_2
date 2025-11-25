using System.Globalization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Views.General
{
    public class TotalBetView : MonoBehaviour
    {
        [SerializeField] private Text _text;
        
        private Tween _counterTween = null;
        
        public void UpdateMoney(int count)
        {
            Debug.Log("$Updated" + count);
            
            if (_counterTween != null && _counterTween.IsActive())
                _counterTween.Kill();

            var format = new NumberFormatInfo { NumberGroupSeparator = " " };

            _counterTween = DOVirtual.Int(int.Parse(_text.text.Replace(" ", "")), count, 0.35f,
                (value) => _text.text = value.ToString("#,0", format)).Play();
        }
    }
}