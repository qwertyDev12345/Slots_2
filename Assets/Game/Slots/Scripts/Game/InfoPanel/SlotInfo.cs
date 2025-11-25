using Slots.Data.Slots;
using UnityEngine;
using UnityEngine.UI;

namespace Slots.Game.InfoPanel
{
    public class SlotInfo : MonoBehaviour
    {
        [SerializeField] private Image _icon = null;
        [SerializeField] private GameObject _fsHeader = null;
        [SerializeField] private InfoLine _infoLine = null;

        public void SetInfo(SlotData data)
        {
            if (data.Type != SlotType.Scatter)
                _fsHeader.SetActive(false);
            
            _icon.sprite = data.SlotSprite;

            foreach (var key in data.Bonuses.Keys)
            {
                InfoLine line = Instantiate(_infoLine, _infoLine.transform.parent);
                
                line.SetInfo(key, data.Bonuses[key]);
            }
            
            _infoLine.gameObject.SetActive(false);
        }
    }
}
