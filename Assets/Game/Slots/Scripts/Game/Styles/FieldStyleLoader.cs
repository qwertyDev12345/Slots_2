using Slots.Data.Styles;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Slots.Game.Styles
{
    public class FieldStyleLoader : MonoBehaviour
    {
        private void Awake() =>
            LoadSkin();

        public void LoadSkin()
        {
            FieldGenerator fieldGenerator = GetComponentInParent<FieldGenerator>();

            StyleSet styleSet = fieldGenerator.GetFieldStyleSet();

            Image image = GetComponent<Image>();

            if (image != null && styleSet != null)
            {
                var set = styleSet.GetImageSet();

                image.sprite = set.image;

#if UNITY_EDITOR

                if (!Application.isPlaying)
                    EditorUtility.SetDirty(image);

#endif
            }
        }
    }
}