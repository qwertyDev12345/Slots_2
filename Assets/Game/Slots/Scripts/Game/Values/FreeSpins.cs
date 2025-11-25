using System;

namespace Slots.Game.Values
{
    public class FreeSpins
    {
        public static event Action<int> OnChanged = null;

        public static int Count
        {
            get => _count;
            set
            {
                _count = value;

                OnChanged?.Invoke(_count);
            }
        }

        private static int _count = 0;
    }
}