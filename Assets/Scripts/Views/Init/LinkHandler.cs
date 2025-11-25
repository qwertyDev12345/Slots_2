using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views.Init
{
    public class LinkHandler : MonoBehaviour, IPointerClickHandler
    {
        private TMP_Text txt;

        public event Action<int> OnPressTextBtnAction;

        void Awake() => txt = GetComponent<TMP_Text>();

        public void OnPointerClick(PointerEventData eventData)
        {
            int i = TMP_TextUtilities.FindIntersectingLink(txt, eventData.position, eventData.pressEventCamera);
            if (i == -1) return;

            var info = txt.textInfo.linkInfo[i];
            string id = info.GetLinkID();

            switch (id)
            {
                case "terms":
                    OnPressTextBtnAction?.Invoke(1);
                    break;
                case "privacy":
                    OnPressTextBtnAction?.Invoke(0);
                    break;
            }
        }
    }
}