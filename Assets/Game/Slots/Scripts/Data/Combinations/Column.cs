using System;
using UnityEngine;

namespace Slots.Data.Combinations
{
    [Serializable]
    public class Column
    {
        [SerializeField] private bool[] _cells = new bool[0];

        public bool[] Cells => _cells;
    }
}