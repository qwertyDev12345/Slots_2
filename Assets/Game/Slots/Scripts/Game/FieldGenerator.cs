using System;
using System.Collections.Generic;
using Slots.Data;
using Slots.Data.Styles;
using Slots.Game.Machine;
using UnityEngine;
using Slots.Game.Styles;
using Slots.Data.Slots;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Slots.Game
{
    public class FieldGenerator : MonoBehaviour
    {
        [SerializeField] private SlotMachineSettings _settings = null;
        [SerializeField] private StylesData _styles = null;
        [SerializeField] private SlotsData _slots = null;

        [SerializeField] private SlotMachine _slotMachine = null;

        [SerializeField] private SlotsLine _slotsLine = null;
        [SerializeField] private List<SlotsLine> _lines = new List<SlotsLine>();

        public Vector2Int Size
        {
            get
            {
                if (_styles == null)
                    throw new NullReferenceException("SlotMachine settings not setuped");

                return _settings.Size;
            }
        }

        public int MaxBet
        {
            get
            {
                if (_styles == null)
                    throw new NullReferenceException("SlotMachine settings not setuped");

                return _settings.MaxBet;
            }
        }

        public List<bool[,]> Combinations
        {
            get
            {
                if (_styles == null)
                    throw new NullReferenceException("Styles data not setuped");

                return _settings.Combinations;
            }
        }

        private void Awake()
        {   
            SetupField();
            
            SetupInfoPanel();
        }

        private void SetupField()
        {
            if (_styles == null)
                throw new NullReferenceException("Styles data not setuped");

            SlotsRandomizer.InitPool(_slots.Slots);

            int removeCount = _lines.Count;

            for (int i = 0; i < removeCount; i++)
            {
                SlotsLine line = _lines[i];

                if (Application.isPlaying)
                    Destroy(line.gameObject);
                else
                    DestroyImmediate(line.gameObject);
            }

            _lines = new List<SlotsLine>();

            if (_slotsLine == null)
                throw new ArgumentException("SlotsLine is missing");

            for (int i = 0; i < _settings.Size.x; i++)
            {
                SlotsLine line = Instantiate(_slotsLine, _slotMachine.transform);

                _lines.Add(line);

#if UNITY_EDITOR

                EditorUtility.SetDirty(line);

#endif

                line.Init(_settings.Size.y);
            }

            _slotMachine.Init(_lines.ToArray());
        }

        private void SetupInfoPanel()
        {
            var panel = GetComponentInChildren<InfoPanel.InfoPanel>(true);
            
            panel.Init(this, Combinations, _slots.Slots);
        }
        
        public StyleSet GetFieldStyleSet()
        {
            if (_styles == null)
                throw new NullReferenceException("Styles data not setuped");

            StyleSet set = null;

            string key = $"Field_{Size.x}x{Size.y}";

            if (_styles.Styles.ContainsKey(key))
                set = _styles.Styles[key];

            return set;
        }

        public StyleSet GetStyleSet(StyleSetGroup group) =>
            GetStyleSet(group.ToString());

        public StyleSet GetStyleSet(string group)
        {
            if (_styles == null)
                throw new NullReferenceException("Styles data not setuped");

            StyleSet set = null;

            string key = group.ToString();

            if (_styles.Styles.ContainsKey(key))
                set = _styles.Styles[key];

            return set;
        }

#if UNITY_EDITOR

        public void PreloadSlotMachineDesign()
        {
            SetupField();

            EditorUtility.SetDirty(this);

            FieldStyleLoader fieldSkinLoader = GetComponentInChildren<FieldStyleLoader>();
            fieldSkinLoader.LoadSkin();

            StyleLoader[] skinLoaders = GetComponentsInChildren<StyleLoader>();

            foreach (StyleLoader skinLoader in skinLoaders)
                skinLoader.LoadSkin();
        }

#endif
    }
}