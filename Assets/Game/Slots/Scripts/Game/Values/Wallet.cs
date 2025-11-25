using System;
using UnityEngine;

namespace Slots.Game.Values
{
    public static class Wallet
    {
        public static event Action<int> OnChangedMoney = null;

        public static int Money
        {
            get => PlayerPrefs.GetInt("WalletMoney", 500);

            private set
            {
                if (value > 999999999 || value < 0)
                    value = 999999999;
                
                PlayerPrefs.SetInt("WalletMoney", value);
                PlayerPrefs.Save();

                OnChangedMoney?.Invoke(value);
            }
        }

        public static void AddMoney(int money)
        {
            if (money > 0)
                Money += money;
        }

        public static bool TryPurchase(int money)
        {
            if (Money >= money)
            {
                Money -= money;

                return true;
            }

            return false;
        }
    }
}