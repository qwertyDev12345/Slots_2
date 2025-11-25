using System;
using Types;
using UnityEngine;
using UnityEngine.UI;

namespace Views.RoyalRoulette
{
    [RequireComponent(typeof(Button))]
    public class BetBtnView : MonoBehaviour
    {
        [Space]
        [SerializeField] private BetBtnType _type;
        [SerializeField] private  int _value;
        
        public BetBtnType Type => _type;
        public int Value => _value;
        
        private Button _btn;
        private CoinView _view;

        public event Action<BetBtnView> OnPressBtn;

        private void OnEnable()
        {
            if (_btn == null)
            {
                _btn = GetComponent<Button>();
            }
            
            _btn.onClick.AddListener(Notification);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveAllListeners();
        }

        public void SetState(bool value)
        {
            _btn ??= GetComponent<Button>();
            
            _btn.interactable = value;

            if (value && _view != null)
            {
                Destroy(_view.gameObject);
            }
        }

        public void SetCoin(CoinView view)
        {
            _view = view;
        }

        public void ChangeBetOnCoin(int value)
        {
            _view.SetCount(value);
        }

        private void Notification()
        {
            OnPressBtn?.Invoke(this);
        }
    }
}