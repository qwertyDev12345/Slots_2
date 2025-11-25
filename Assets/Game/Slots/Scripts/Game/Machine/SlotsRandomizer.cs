using System.Linq;
using Slots.Data.Slots;
using UnityEngine;

namespace Slots.Game.Machine
{
    public static class SlotsRandomizer
    {
        private static SlotData[] _slots = null;

        public static void InitPool(SlotData[] slots) =>
            _slots = slots;

        public static SlotData[] GetPool() =>
            _slots;

        public static SlotData GetRandomSlot()
        {
            float maxValue = SpawnRateTo(null);

            float randomValue = Random.Range(0f, maxValue);

            return _slots.Where(s => randomValue <= SpawnRateTo(s)).First();
        }

        private static float SpawnRateTo(SlotData slot)
        {
            float rate = 0f;

            for (int i = 0; i < _slots.Length; i++)
            {
                rate += _slots[i].SpawnPercent;

                if (_slots[i] == slot)
                    return rate;
            }

            return rate;
        }
    }
}