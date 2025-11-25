using System;
using UnityEngine;

namespace Slots.Data.Combinations
{
    [Serializable]
    public class CombinationInfo
    {
        [SerializeField] private Column[] _columns = new Column[0];

        public bool[,] Combination
        {
            get
            {
                if (_columns.Length == 0)
                    return new bool[0, 0];

                bool[,] combination = new bool[_columns.Length, _columns[0].Cells.Length];

                for (int x = 0; x < _columns.Length; x++)
                {
                    for (int y = 0; y < _columns[x].Cells.Length && y < _columns[0].Cells.Length; y++)
                    {
                        combination[x, y] = _columns[x].Cells[y];
                    }
                }

                return combination;
            }
        }
    }
}