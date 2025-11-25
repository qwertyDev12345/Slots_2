using System.Globalization;
using DG.Tweening;
using Slots.Game.Values;
using UnityEngine;
using UnityEngine.UI;

namespace Views.General
{
    public class BalanceDisplay : MonoBehaviour
    {
        private Text _text = null;

        private Tween _counterTween = null;

        private void Awake()
        {
            var format = new NumberFormatInfo { NumberGroupSeparator = " " };

            _text = GetComponent<Text>();
            _text.text = Wallet.Money.ToString("#,0", format);

            Wallet.OnChangedMoney += UpdateMoney;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Plus))
                Wallet.AddMoney(100);

            if (Input.GetKeyDown(KeyCode.Minus))
            {
                if (!Wallet.TryPurchase(100))
                    Wallet.TryPurchase(Wallet.Money);
            }
        }

        private void OnDestroy()
        {
            Wallet.OnChangedMoney -= UpdateMoney;
        }

        private void PlayNoMoneyAnim()
        {
            transform.parent.DOKill();
            transform.parent.DOScale(1f, 0.1f).OnComplete(() =>
            {
                transform.parent.DOScale(1.05f, 0.2f).SetLoops(6, LoopType.Yoyo);
            });
        }

        private void UpdateMoney(int count)
        {
            if (_counterTween != null && _counterTween.IsActive())
                _counterTween.Kill();

            var format = new NumberFormatInfo { NumberGroupSeparator = " " };

            _counterTween = DOVirtual.Int(int.Parse(_text.text.Replace(" ", "")), count, 0.35f,
                (value) => _text.text = value.ToString("#,0", format)).Play();
        }
    }
}