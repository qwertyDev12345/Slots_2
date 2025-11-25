using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slots.Data.Styles
{
    [CreateAssetMenu(fileName = "NewStylesData", menuName = "Slots/Add new styles data", order = 1)]
    public class StylesData : ScriptableObject
    {
        [SerializeField] private string[] _groups = new string[0];
        [SerializeField] private StyleSet[] _sets = new StyleSet[0];

        public Dictionary<string, StyleSet> Styles => _groups
            .Zip(_sets, (k, v) => new { k, v })
            .ToDictionary(x => x.k, x => x.v);
    }
}