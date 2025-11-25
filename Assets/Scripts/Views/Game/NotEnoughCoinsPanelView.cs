using Tools.Core.UnityAdsService.Scripts;
using UnityEngine;
using Views.General;

namespace Views.Game
{
    public class NotEnoughCoinsPanelView : PanelView
    {
        [SerializeField] 
        private UnityAdsButton _adBtn;

        private void Awake()
        {
            _adBtn.OnCanGetReward += delegate { Notification(1); };
        }
    }
}