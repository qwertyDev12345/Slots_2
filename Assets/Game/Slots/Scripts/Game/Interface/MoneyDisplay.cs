using System;
using UnityEngine;
using DG.Tweening;
using System.Globalization;
using UnityEngine.UI;
using Slots.Game.Values;

namespace Slots.Game.Interface
{
    [RequireComponent(typeof(Text))]
    public class MoneyDisplay : MonoBehaviour
    {
        private Text _text = null;

        private Tween _counterTween = null;

        private SlotMachineEvents _events = null;

        private void Awake()
        {
            _events = GetComponentInParent<SlotMachineEvents>();
            
            _events.OnEndMoney.AddListener(PlayNoMoneyAnim);
            
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
            _events.OnEndMoney.RemoveListener(PlayNoMoneyAnim);
            
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