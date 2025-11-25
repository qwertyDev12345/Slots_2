using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Slots.Data.Slots
{
    [Serializable]
    public class SlotData
    {
        [SerializeField] private SlotType _type = SlotType.Standart;

        [SerializeField] private Sprite _slotSprite = null;
        [SerializeField] private Sprite _glowSprite = null;

        [SerializeField] private float _spawnPercent = 10f;

        [SerializeField] private int[] _inLineCount = new int[0];
        [SerializeField] private float[] _inLinePrize = new float[0];

        public SlotType Type => _type;

        public Sprite SlotSprite => _slotSprite;
        public Sprite GlowSprite => _glowSprite;

        public float SpawnPercent => _spawnPercent;

        public Dictionary<int, float> Bonuses => _inLineCount
            .Zip(_inLinePrize, (k, v) => new { k, v })
            .ToDictionary(x => x.k, x => x.v);
    }
}