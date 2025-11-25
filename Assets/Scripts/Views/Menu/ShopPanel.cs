using System;
using Slots.Game.Values;
using Tools.Core.UnityAdsService.Scripts;
using UnityEngine;
using Views.General;

namespace Views.Menu
{
    public class ShopPanel : PanelView
    {
        [SerializeField] private UnityAdsButton _adBtn;

        private void Awake()
        {
            _adBtn.OnCanGetReward += OnCanGetRewardForAd;
        }

        private void OnDestroy()
        {
            _adBtn.OnCanGetReward -= OnCanGetRewardForAd;
        }

        public void OnSuccess1000Coins()
        {
            Wallet.AddMoney(1000);
        }
        
        public void OnSuccess3000Coins()
        {
            Wallet.AddMoney(3000);
        }
        
        public void OnSuccess5000Coins()
        {
            Wallet.AddMoney(5000);
        }

        private void OnCanGetRewardForAd()
        {
            Wallet.AddMoney(100);
        }
    }
}