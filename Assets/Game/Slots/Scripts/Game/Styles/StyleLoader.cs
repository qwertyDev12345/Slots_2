using Slots.Data.Styles;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Slots.Game.Styles
{
    public class StyleLoader : MonoBehaviour
    {
        [SerializeField] private string _styleGroupKey = "None";

        private void Awake() =>
            LoadSkin();

        public void LoadSkin()
        {
            FieldGenerator fieldGenerator = GetComponentInParent<FieldGenerator>();

            StyleSet styleSet = fieldGenerator.GetStyleSet(_styleGroupKey);

            if (styleSet == null)
                return;

            switch (styleSet.Type)
            {
                case StyleSetType.Text:

                    Text text = GetComponent<Text>();

                    if (text != null)
                    {
                        var set = styleSet.GetTextSet();

                        text.font = set.font;
                        text.fontSize = set.size;
                        text.color = set.color;

                        if (text.resizeTextForBestFit)
                        {
                            text.resizeTextMaxSize = set.size;
                            
                            float width = text.rectTransform.sizeDelta.x;
                            float prefferedHeight = text.preferredHeight;

                            text.rectTransform.sizeDelta = new Vector2(width, prefferedHeight);
                        }
#if UNITY_EDITOR

                        if (!Application.isPlaying)
                            EditorUtility.SetDirty(text);

#endif
                    }

                    break;

                case StyleSetType.Image:

                    Image image = GetComponent<Image>();

                    if (image != null)
                    {
                        var set = styleSet.GetImageSet();

                        image.sprite = set.image;

                        if (image.sprite == null)
                                image.color = new Color(0f, 0f, 0f, 0f);
#if UNITY_EDITOR

                        if (!Application.isPlaying)
                            EditorUtility.SetDirty(image);

#endif
                    }

                    break;

                case StyleSetType.Button:

                    Button button = GetComponent<Button>();

                    if (button != null)
                    {
                        var set = styleSet.GetButtonSet();

                        button.transition = Selectable.Transition.SpriteSwap;

                        button.image.sprite = set.normal;

                        SpriteState spriteState = new SpriteState();

                        spriteState.selectedSprite = set.normal;
                        spriteState.highlightedSprite = set.normal;
                        spriteState.pressedSprite = set.pressed;
                        spriteState.disabledSprite = set.disabled;

                        button.spriteState = spriteState;
#if UNITY_EDITOR

                        if (!Application.isPlaying)
                        {
                            EditorUtility.SetDirty(button);
                            EditorUtility.SetDirty(button.image);
                        }

#endif
                    }

                    break;

                case StyleSetType.Toggle:

                    Toggle toggle = GetComponent<Toggle>();

                    if (toggle != null)
                    {
                        var set = styleSet.GetToggleSet();

                        toggle.transition = Selectable.Transition.SpriteSwap;

                        toggle.image.sprite = set.normal;
                        
                        (toggle.graphic as Image).sprite = set.activated;

                        SpriteState spriteState = new SpriteState();

                        spriteState.selectedSprite = set.normal;
                        spriteState.highlightedSprite = set.normal;
                        spriteState.pressedSprite = set.pressed;
                        spriteState.disabledSprite = set.disabled;

                        toggle.spriteState = spriteState;
#if UNITY_EDITOR

                        if (!Application.isPlaying)
                        {
                            EditorUtility.SetDirty(toggle);
                            EditorUtility.SetDirty(toggle.targetGraphic);
                            EditorUtility.SetDirty(toggle.graphic);
                        }

#endif
                    }

                    break;
            }
        }
    }
}