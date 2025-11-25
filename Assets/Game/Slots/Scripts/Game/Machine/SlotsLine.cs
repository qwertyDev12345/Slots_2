using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Slots.Game.Machine
{
    public class SlotsLine : MonoBehaviour
    {
        [SerializeField] private VerticalLayoutGroup _group = null;
        [SerializeField] private ContentSizeFitter _scaler = null;

        [Space]

        [SerializeField] private List<Slot> _slotsLine = new List<Slot>();

        private int _count = 0;
        private float _itemHeight = 0f;

        private float _lineHeight = 0f;
        private float _minVertical = 0f;

        public Slot[] TargetSlots
        {
            get
            {
                Slot[] slots = new Slot[_count];

                for (int i = 0; i < _count; i++)
                    slots[i] = _slotsLine[i + 1];

                return slots;
            }
        }

        public void Init(int count)
        {
            _count = count;

            if (_slotsLine.Count == 0)
                throw new ArgumentException("Slot is missing");

            _slotsLine[0].SetSlot(SlotsRandomizer.GetRandomSlot());

            _slotsLine[0].Init();

            _itemHeight = _slotsLine[0].RectTransform.sizeDelta.y;

            for (int i = 1; i < _count + 2; i++)
            {
                Slot slot = Instantiate(_slotsLine[0], _slotsLine[0].transform.parent);

                _slotsLine.Add(slot);

                slot.gameObject.name = "Slot_" + i;

#if UNITY_EDITOR

                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(slot);
                    EditorUtility.SetDirty(slot.gameObject);
                }

#endif

                slot.SetSlot(SlotsRandomizer.GetRandomSlot());
                slot.Init();
            }
        }

        public void StartSpin(Action onSpinnedCallback)
        {
            if (_group.enabled)
            {
                RectTransform rectTransform = GetComponent<RectTransform>();

                Vector2 size = rectTransform.sizeDelta;

                _lineHeight = size.y;
                _minVertical = -(_lineHeight / 2f);

                _scaler.enabled = false;
                _group.enabled = false;

                rectTransform.sizeDelta = size;
            }

            foreach (Slot slot in _slotsLine)
                slot.LifeCount--;

            float lastOffset = 0f;

            float randomOffset = _lineHeight * 3f;

            DOVirtual.Float(0, randomOffset, 0.5f, (offset) =>
            {
                float acceleration = offset - lastOffset;

                lastOffset = offset;

                List<Slot> slotsForMove = new List<Slot>();

                for (int i = _slotsLine.Count() - 1; i >= 0; i--)
                {
                    Slot slot = _slotsLine[i];

                    RectTransform rectTransform = slot.RectTransform;

                    rectTransform.localPosition += Vector3.down * acceleration;

                    if (rectTransform.localPosition.y < _minVertical)
                        slotsForMove.Add(slot);
                }

                foreach (Slot slot in slotsForMove)
                {
                    RectTransform rectTransform = slot.RectTransform;

                    rectTransform.localPosition += Vector3.up * _lineHeight;

                    slot.SetSlot(SlotsRandomizer.GetRandomSlot());

                    _slotsLine.Remove(slot);
                    _slotsLine.Insert(0, slot);

                    rectTransform.SetSiblingIndex(0);
                }
            }).OnComplete(() => onSpinnedCallback?.Invoke()).Play();
        }
    }
}