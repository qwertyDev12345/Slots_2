using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Slots.Data.Slots;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Slots.Game.Machine
{
    public class Slot : MonoBehaviour
    {
        private const int WILD_LIFE_COUNT = 2;

        [SerializeField] private Image _slotImage = null;
        [SerializeField] private Image _glowImage = null;

        public RectTransform RectTransform { get; private set; }

        public SlotData SlotData { get; private set; }

        public int LifeCount { get; set; } = 0;

        public void Init()
        {
            RectTransform = GetComponent<RectTransform>();

            if (SlotData.Type == SlotType.Wild)
                LifeCount = WILD_LIFE_COUNT;
            else
                LifeCount = 1;
        }

        public void SetSlot(SlotData slot)
        {
            if (LifeCount > 0)
                return;

            SlotData = slot;

            if (SlotData.Type == SlotType.Wild)
                LifeCount = WILD_LIFE_COUNT;
            else
                LifeCount = 1;

            _slotImage.sprite = SlotData.SlotSprite;
            _glowImage.sprite = SlotData.GlowSprite;

#if UNITY_EDITOR

            if (!Application.isPlaying)
                EditorUtility.SetDirty(_slotImage);

#endif
        }

        public void PlayLineAnim()
        {
            if (_glowImage != null && _glowImage.IsActive())
                _glowImage.DOKill();

            _glowImage.DOFade(1f, 0.25f).Play();
        }

        public void HideLineAnim()
        {
            if (_glowImage != null && _glowImage.IsActive())
                _glowImage.DOKill();

            _glowImage.DOFade(0f, 0.25f).Play();
        }
    }
}