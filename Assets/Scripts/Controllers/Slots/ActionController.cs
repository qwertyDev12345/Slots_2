using System;
using UnityEngine;

namespace Controllers.Slots
{
    public class ActionController : MonoBehaviour
    {
        public event Action OnPressHomeBtnAction;
        public event Action OnEndMoneyAction;
        public event Action OnPressBtnAction;
        public event Action OnWinLineAction;
        public event Action OnSpinAction;

        public void OnPressHomeBtn()
        {
            OnPressHomeBtnAction?.Invoke();
        }
        
        public void OnEndMoney()
        {
            OnEndMoneyAction?.Invoke();
        }

        public void OnPressBtn()
        {
            OnPressBtnAction?.Invoke();
        }

        public void OnWinLine()
        {
            OnWinLineAction?.Invoke();
        }

        public void OnSpin()
        {
            OnSpinAction?.Invoke();
        }
    }
}