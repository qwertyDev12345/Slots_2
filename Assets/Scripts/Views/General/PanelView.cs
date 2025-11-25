using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Views.General
{
    public class PanelView : MonoBehaviour
    {
        public Action<int> PressBtnAction { get; set; }

        [SerializeField]
        private List<Button> _btns;
        [SerializeField] 

        protected List<Button> Btns => _btns;
        
        private bool _isAniming = false;

        private void OnEnable()
        {
            for (int i = 0; i < _btns.Count; i++)
            {
                int index = i;

                _btns[i].onClick.AddListener(() =>Notification(index));
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _btns.Count; i++)
            {
                int index = i;

                _btns[i].onClick.RemoveAllListeners();
            }
        }

        protected void Notification(int index)
        {
            PressBtnAction?.Invoke(index);
        }
        
        public void Show()
        {
            if (_isAniming)
                return;

            _isAniming = true;
            
            gameObject.SetActive(true);

            transform.DOScale(1f, 0.25f).OnComplete(() =>
            {
                _isAniming = false;
            }).Play();
        }

        public void Hide()
        {
            if (_isAniming)
                return;

            _isAniming = true;

            transform.localScale = Vector3.one;
            

            transform.DOScale(0f, 0.25f).OnComplete(() =>
            {
                _isAniming = false;
                
                transform.localScale = Vector3.zero;
            
                gameObject.SetActive(false);
            }).Play();
        }
    }
}