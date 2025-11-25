using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Slots.Data.Slots;
using Slots.Data.Styles;
using Slots.Swipes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Slots.Game.InfoPanel
{
    public class InfoPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _fade = null;
        [SerializeField] private RectTransform _panel = null;

        [Space]

        [SerializeField] private UnityEvent _onShow = null;
        [SerializeField] private UnityEvent _onHide = null;

        [Space] 
        
        [Header("Panel")]
        
        [Space] 

        [SerializeField] private Image[] _points = new Image[2];

        [Space] 
        
        [SerializeField] private RectTransform _firstPanel = null;
        [SerializeField] private RectTransform _secondPanel = null;

        [Space] 
        
        [Header("Combbination")]
        
        [Space] 

        [SerializeField] private ScrollRect _combinationsScrollRect = null;
        
        [Space] 

        [SerializeField] private GridLayoutGroup _content = null;
        [SerializeField] private GridLayoutGroup _combination = null;
        
        [Space] 
        
        [Header("Slots")]
        
        [Space] 
        
        [Space] 

        [SerializeField] private ScrollRect _slotsScrollRect = null;
        
        [Space] 

        [SerializeField] private SlotInfo _slotInfoPrefab = null;
        
        [Space] 

        private int _currentIndex = 0;

        private bool _isAniming = false;
        
        public void Init(FieldGenerator field, List<bool[,]> combinations, SlotData[] slots)
        {
            SpawnCombinations(field, combinations);

            SpawnSlotsInfo(slots);
            
            gameObject.SetActive(false);
        }

        private void SpawnCombinations(FieldGenerator field, List<bool[,]> combinations)
        {
            Sprite onCellSprite = field.GetStyleSet(StyleSetGroup.InfoPanel_Combination_Active_Cell)
                .GetImageSet().image;
            
            Sprite offCellSprite = field.GetStyleSet(StyleSetGroup.InfoPanel_Combination_Disabled_Cell)
                .GetImageSet().image;
        
            int sizeX = combinations[0].GetLength(0);
            int sizeY = combinations[0].GetLength(1);

            _content.cellSize = new Vector2(
                (sizeX * _combination.cellSize.x) + ((sizeX - 1) * _combination.spacing.x) + _combination.padding.left + _combination.padding.right,
                (sizeY * _combination.cellSize.y) + ((sizeY - 1) * _combination.spacing.y)  + _combination.padding.top + _combination.padding.bottom
            );
            
            foreach (var combination in combinations)
            {
                Transform newCombination = Instantiate(_combination, _content.transform).transform;

                for (int y = 0; y < sizeY; y++)
                {
                    for (int x = 0; x < sizeX; x++)
                    {
                        GameObject cell = new GameObject("cell");

                        cell.transform.SetParent(newCombination);
                        cell.transform.localScale = Vector3.one;
                        
                        Image cellImage = cell.AddComponent<Image>();

                        if (combination[x, y])
                            cellImage.sprite = onCellSprite;
                        else
                            cellImage.sprite = offCellSprite;
                    }
                }
            }
            
            _combination.gameObject.SetActive(false);

            _combinationsScrollRect.verticalNormalizedPosition = 1f;
        }

        private void SpawnSlotsInfo(SlotData[] slots)
        {
            SlotData[] standartSlots = slots.Where(s => s.Type == SlotType.Standart).ToArray();

            for (int i = 0; i < standartSlots.Length; i++)
            {
                SlotInfo info = Instantiate(_slotInfoPrefab, _slotInfoPrefab.transform.parent);
                
                info.SetInfo(standartSlots[i]);
            }

            SlotData[] fsSlots = slots.Where(s => s.Type == SlotType.Scatter).ToArray();
            
            for (int i = 0; i < fsSlots.Length; i++)
            {
                SlotInfo info = Instantiate(_slotInfoPrefab, _slotInfoPrefab.transform.parent);
                
                info.SetInfo(fsSlots[i]);
            }

            _slotInfoPrefab.gameObject.SetActive(false);

            _slotsScrollRect.verticalNormalizedPosition = 1f;
        }
        
        private void OnEnable() =>
            SwipesInput.OnSwiped += SwipesInputOnOnSwiped;
        
        private void OnDisable() =>
            SwipesInput.OnSwiped -= SwipesInputOnOnSwiped;

        private void SwipesInputOnOnSwiped(SwipeDirection direction)
        {
            switch (direction)
            {
                case SwipeDirection.Right:
                    
                    if (_currentIndex == 0)
                        return;
                    
                    MoveTo(0);

                    break;
                
                case SwipeDirection.Left:
                    
                    if (_currentIndex == 1)
                        return;

                    MoveTo(1);
                    
                    break;
            }
        }

        private void MoveTo(int index)
        {
            _currentIndex = index;

            for (int i = 0; i < _points.Length; i++)
            {
                _points[i].DOKill();
                _points[i].DOFade(i == index ? 1f : 0f, 0.25f).Play();
            }

            if (index == 0)
            {
                _firstPanel.DOKill();
                _firstPanel.DOAnchorPosX(0f, 0.25f).Play();
                
                _secondPanel.DOKill();
                _secondPanel.DOAnchorPosX(_secondPanel.sizeDelta.x, 0.25f).Play();
            }
            else
            {
                _firstPanel.DOKill();
                _firstPanel.DOAnchorPosX(-_firstPanel.sizeDelta.x, 0.25f).Play();
                
                _secondPanel.DOKill();
                _secondPanel.DOAnchorPosX(0f, 0.25f).Play();
                _secondPanel.DOAnchorPosX(0f, 0.25f).Play();
            }
        }

        public void Show()
        {
            if (_isAniming)
                return;

            _onShow?.Invoke();

            _isAniming = true;

            _panel.localScale = Vector3.zero;

            if(_fade != null)
                _fade.alpha = 0f;

            gameObject.SetActive(true);

            if (_fade != null)
                _fade.DOFade(1f, 0.25f).SetEase(Ease.Linear).Play();

            _panel.DOScale(1f, 0.25f).OnComplete(() =>
            {
                _isAniming = false;
            }).Play();
        }

        public void Hide()
        {
            if (_isAniming)
                return;

            _isAniming = true;

            _panel.localScale = Vector3.one;

            if (_fade != null)
            {
                _fade.alpha = 1f;
                _fade.DOFade(0f, 0.25f).SetEase(Ease.Linear).Play();
            }

            _panel.DOScale(0f, 0.25f).OnComplete(() =>
            {
                _isAniming = false;

                _onHide?.Invoke();
            
                gameObject.SetActive(false);
            }).Play();
        }
    }
}
