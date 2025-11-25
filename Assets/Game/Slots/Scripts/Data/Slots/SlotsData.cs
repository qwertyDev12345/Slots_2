using UnityEngine;

namespace Slots.Data.Slots
{
    [CreateAssetMenu(fileName = "NewSlotsData", menuName = "Slots/Add new slots data", order = 2)]
    public class SlotsData : ScriptableObject
    {
        [SerializeField] private SlotData[] _slots = new SlotData[0];

        public SlotData[] Slots => _slots;
    }
}