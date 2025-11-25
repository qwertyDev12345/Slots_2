using System.Collections.Generic;
using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "Config", menuName = "Roulette Config", order = 0)]
    public class RouletteConfig : ScriptableObject
    {
        [Tooltip("Номера красных ячеек")] public List<int> redNumbers = new List<int>();
        
        [Tooltip("Номера черных ячеек")]
        public List<int> blackNumbers = new List<int>();

        public bool IsRed(int number)
        {
            return redNumbers.Contains(number);
        }

        public bool IsBlack(int number)
        {
            return blackNumbers.Contains(number);
        }
    }
}