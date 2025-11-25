using System;
using UnityEngine;

namespace Slots.Data.Styles
{
    [Serializable]
    public class StyleSet
    {
        [SerializeField] private StyleSetType _type = StyleSetType.Image;

        [SerializeField] private Font _font = null;
        [SerializeField] private float _value = 0f;
        [SerializeField] private Color _color = new Color(1f, 1f, 1f, 1f);
        [SerializeField] private Color[] _colors = new Color[0];
        [SerializeField] private Sprite _sprite_0 = null;
        [SerializeField] private Sprite _sprite_1 = null;
        [SerializeField] private Sprite _sprite_2 = null;
        [SerializeField] private Sprite _sprite_3 = null;

        public StyleSetType Type => _type;

        public struct ImageSet
        {
            public Sprite image;
        }

        public struct TextSet
        {
            public Font font;
            public int size;
            public Color color;
        }

        public struct ButtonSet
        {
            public Sprite normal;
            public Sprite pressed;
            public Sprite disabled;
        }

        public struct ToggleSet
        {
            public Sprite normal;
            public Sprite pressed;
            public Sprite disabled;
            public Sprite activated;
        }

        public struct LineRendererSet
        {
            public Sprite sprite;
            public float width;
            public Color[] colors;
        }

        public ImageSet GetImageSet()
        {
            return new ImageSet() { image = _sprite_0 };
        }

        public TextSet GetTextSet()
        {
            return new TextSet()
            {
                font = _font,
                size = (int)_value,
                color = _color,
            };
        }

        public ButtonSet GetButtonSet()
        {
            return new ButtonSet()
            {
                normal = _sprite_0,
                pressed = _sprite_1,
                disabled = _sprite_2,
            };
        }

        public ToggleSet GetToggleSet()
        {
            return new ToggleSet()
            {
                normal = _sprite_0,
                pressed = _sprite_1,
                disabled = _sprite_2,
                activated = _sprite_3,
            };
        }

        public LineRendererSet GetLineRendererSet()
        {
            return new LineRendererSet()
            {
                sprite = _sprite_0,
                colors = _colors,
            };
        }
    }
}