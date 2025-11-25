using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Views.Settings
{
    public class SwitcherBtn : MonoBehaviour
    {
        public Action PressBtnAction
        {
            get;
            set;
        }

        [SerializeField] 
        private Button _btn;
        [SerializeField] 
        private Image _image;
        [SerializeField] 
        private List<Sprite> _sprites;

        private void OnEnable()
        {
            _btn.onClick.AddListener(NotificationPressBtn);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveAllListeners();
        }

        public void SetSprite(int value)
        {
            _image.sprite = _sprites[value];
        }

        private void  NotificationPressBtn()
        {
            PressBtnAction.Invoke();
        }
    }
}