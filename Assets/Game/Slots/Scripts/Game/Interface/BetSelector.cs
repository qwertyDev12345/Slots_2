using System;
using System.Linq;
using DG.Tweening;
using Slots.Game.Values;
using UnityEngine;
using UnityEngine.UI;

namespace Slots.Game.Interface
{
    public class BetSelector : MonoBehaviour
    {
        [SerializeField] private int _startBet = 25;

        [Space]

        [SerializeField] private Text _betText = null;

        [Space]

        [SerializeField] private Button _addButton = null;

        [Space]

        [SerializeField] private Button _minusButton = null;

        [Space]

        [SerializeField] private Button _maxButton = null;

        [Space]

        [SerializeField] private int[] _betSteps = new int[0];

        private int _currentBet = 1;

        private Tween _counterTween = null;

        private FieldGenerator _fieldGenerator = null;
        
        private SlotMachineEvents _events = null;

        private bool _isActive = true;
        
        public int CurrentBet
        {
            get => _currentBet;

            private set
            {
                _currentBet = value;

                if (!ValidateBet())
                    return;

                if (_counterTween != null && _counterTween.IsActive())
                    _counterTween.Kill();

                _counterTween = DOVirtual.Int(int.Parse(_betText.text), _currentBet, 0.35f,
                    (value) => _betText.text = value.ToString()).Play();
            }
        }

        private int CurrentStepIndex
        {
            get
            {
                if (CurrentBet == 0)
                    return 0;
                
                int currentStep = _betSteps.Where(s => s <= CurrentBet).Last();

                return Array.IndexOf(_betSteps, currentStep);
            }
        }

        private void Awake()
        {
            _fieldGenerator = GetComponentInParent<FieldGenerator>();
            
            _events = GetComponentInParent<SlotMachineEvents>();
            
            CurrentBet = _startBet;

            _addButton.onClick.AddListener(() =>
            {
                if (Wallet.Money == 0)
                {
                    _events.CallEndMoney();
                }
                
                if (CurrentBet != Wallet.Money)
                {
                    if (CurrentBet == _betSteps[CurrentStepIndex] && CurrentStepIndex < _betSteps.Length - 1)
                        CurrentBet = _betSteps[CurrentStepIndex + 1];
                    else
                    {
                        int counts = CurrentBet / _betSteps[CurrentStepIndex] + 1;

                        CurrentBet = _betSteps[CurrentStepIndex] * counts;
                    }
                }
                
                UpdateButtonsState();
            });

            _minusButton.onClick.AddListener(() =>
            {
                if (Wallet.Money == 0)
                {
                    _events.CallEndMoney();
                }

                if (CurrentBet > _betSteps[0])
                {
                    if (CurrentBet > _betSteps[CurrentStepIndex])
                    {
                        int additionals = CurrentBet % _betSteps[CurrentStepIndex];

                        if (additionals != 0)
                            CurrentBet -= additionals;
                        else
                            CurrentBet -= _betSteps[CurrentStepIndex];
                    }
                    else
                        CurrentBet = _betSteps[CurrentStepIndex - 1];
                }
                
                UpdateButtonsState();
            });

            _maxButton.onClick.AddListener(() =>
            {
                if (Wallet.Money == 0)
                {
                    _events.CallEndMoney();
                }

                CurrentBet = Wallet.Money;
                
                UpdateButtonsState();
            });
            
            Wallet.OnChangedMoney += UpdateBetByMoney;
            
            UpdateButtonsState();
        }

        private void OnEnable()
        {
            UpdateBet();
        }

        private void OnDestroy()
        {
            Wallet.OnChangedMoney -= UpdateBetByMoney;
        }

        private void UpdateBetByMoney(int currentMoney)
        {
            if (!_isActive)
                return;

            UpdateBet();
        }

        private bool ValidateBet()
        {
            if (CurrentBet > Wallet.Money)
            {
                CurrentBet = Wallet.Money;

                UpdateButtonsState();
                
                return false;
            }

            if (CurrentBet > _fieldGenerator.MaxBet) 
            {
                CurrentBet = _fieldGenerator.MaxBet;

                UpdateButtonsState();

                return false;
            }
            
            return true;
        }

        private void UpdateButtonsState()
        {
            if (!_isActive)
                return;

            _minusButton.interactable = CurrentBet > 1 || Wallet.Money == 0;
            _addButton.interactable = _currentBet < Mathf.Min(_fieldGenerator.MaxBet, Wallet.Money) || Wallet.Money == 0;
            _maxButton.interactable = _currentBet < Mathf.Min(_fieldGenerator.MaxBet, Wallet.Money) || Wallet.Money == 0;
        }

        public void UpdateBet()
        {
            if (CurrentBet == 0)
            {
                if (Wallet.Money >= _betSteps.First())
                    CurrentBet = _betSteps.First();
            }
            else
            {
                if (CurrentBet > Wallet.Money)
                    CurrentBet = Wallet.Money;
            }
            
            UpdateButtonsState();
        }
        
        public void SetActive(bool active)
        {
            _isActive = active;
            
            _addButton.interactable = active;
            _minusButton.interactable = active;
            _maxButton.interactable = active;

            UpdateButtonsState();
        }
    }
}