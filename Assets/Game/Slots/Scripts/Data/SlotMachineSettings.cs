using System.Collections.Generic;
using System.Linq;
using Slots.Data.Combinations;
using UnityEngine;

namespace Slots.Data
{
    [CreateAssetMenu(fileName = "NewSlotMachineSettings", menuName = "Slots/Add new slot machine settings", order = 0)]
    public class SlotMachineSettings : ScriptableObject
    {
        [SerializeField] private Vector2Int _fieldSize = new Vector2Int(5, 3);
        [SerializeField] private int _maxBet = 1000;
        
        [SerializeField] private CombinationInfo[] _combinations = new CombinationInfo[0];

        public Vector2Int Size => _fieldSize;
        public int MaxBet => _maxBet;
        public List<bool[,]> Combinations => _combinations.Select(c => c.Combination).ToList();
    }
}