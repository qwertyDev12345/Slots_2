using System;
using Slots.Game.Values;
using Types;

namespace Models.Scenes
{
    public class GoldMineRushSceneModel
    {
        static readonly Random _rnd = new Random();
        
        public GoldMineCrashState GetStartState()
        {
            return Wallet.Money > 0 ? GoldMineCrashState.StartState : GoldMineCrashState.NoMoneyState;
        }

        public float GetReward(int bet, float multiplayer)
        {
            return bet * multiplayer;
        }
        
        public float GetCrashValue(float rate = 0.25f, float minX = 5f, float maxX = 100.0f)
        {
            double u = _rnd.NextDouble();
            double x = -Math.Log(1 - u) / rate;
            
            x += minX;
            
            return Math.Min((float)x, maxX);
        }
    }
}