using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Slots.Game.Interface
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class Tiler : MonoBehaviour
    {
        [SerializeField] private bool _useBaseSize = false;
        [SerializeField] private Vector2 _baseSize = new Vector2Int();
        
        private Image _image = null;

        private void Awake()
        {
            _image = GetComponent<Image>();
            
            UpdateImageTile();
        }

        private void Update()
        {
            if (Application.isEditor)
                UpdateImageTile();
        }

        private void UpdateImageTile()
        {
            float pixelsPerUnitMultiplier = 0f;

            Sprite sprite = _image.sprite;

            RectTransform rect = _image.rectTransform;

            Vector2 rectSize;

            if (_useBaseSize)
                rectSize = _baseSize;
            else
                rectSize = rect.sizeDelta;

            if (rectSize.y < rectSize.x)
            {
                int originalHeight = (int)((sprite.rect.height / sprite.pixelsPerUnit) * 100f);

                pixelsPerUnitMultiplier = originalHeight / rectSize.y;
            }
            else
            {
                int originalWidth = (int)((sprite.rect.width / sprite.pixelsPerUnit) * 100f);

                pixelsPerUnitMultiplier = originalWidth / rectSize.x; 
            }

            _image.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
        }
    }
}