using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views.Menu
{
    public class DailyBalloonBtn : MonoBehaviour
    {
        [SerializeField] private Button _btn;
        [SerializeField] private TextMeshProUGUI _text;

        public event Action OnPressBtn;

        private void OnEnable()
        {
            _btn.onClick.AddListener(Notification);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveAllListeners();
        }

        public void SetState(bool isActive)
        {
            _btn.interactable = isActive;
            _text.enabled = !isActive;
        }
        
        public void SetTimer(TimeSpan remaining)
        {
            if (remaining.TotalSeconds <= 0)
            {
                SetState(true);
                return;
            }

            int totalHours = remaining.Days * 24 + remaining.Hours;
            _text.text = $"{totalHours:00}H {remaining.Minutes:00}M";
        }

        private void Notification()
        {
            OnPressBtn?.Invoke();
        }
    }
}