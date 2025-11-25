using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Slots.Game.Interface;
using Slots.Game.Values;
using UnityEngine;
using UnityEngine.UI;

namespace Slots.Game.Machine
{
    [RequireComponent(typeof(BonusCalculator))]
    public class SlotMachine : MonoBehaviour
    {
        [SerializeField] private WinDisplay _winDisplay = null;

        [Space]

        [SerializeField] private Button _exitButton = null;
        [SerializeField] private Button _infoButton = null;
        [SerializeField] private Button _spinButton = null;
        [SerializeField] private Toggle _autoSpinToggle = null;

        [Space]

        [SerializeField] private BetSelector _betSelector = null;

        [Space]

        [SerializeField] private FreeSpinsButton _freeSpinsButton = null;

        [Space]

        [SerializeField] private LineDrawer _lineDrawer = null;

        private SlotsLine[] _slotLines = new SlotsLine[0];

        private bool _isSpining = false;

        private BonusCalculator _calculator = null;

        private SlotMachineEvents _events = null;

        public void Init(SlotsLine[] slotLines)
        {
            _calculator = GetComponent<BonusCalculator>();

            _events = GetComponentInParent<SlotMachineEvents>();

            _slotLines = slotLines;

            _spinButton.onClick.AddListener(() => TryNormalSpin());

            _autoSpinToggle.onValueChanged.AddListener((active) =>
            {
                if (!active)
                    return;

                TryNormalSpin();
            });

            _freeSpinsButton.OnPressed += () =>
            {
                if (_isSpining)
                    return;

                _exitButton.interactable = false;
                _infoButton.interactable = false;
                _spinButton.interactable = false;
                _betSelector.SetActive(false);
                _autoSpinToggle.interactable = false;

                StartCoroutine(Spin(true, 50));
            };
        }

        private void TryNormalSpin()
        {
            if (_isSpining)
                return;

            if (Wallet.Money == 0)
            {
                _events.CallEndMoney();
                
                _autoSpinToggle.isOn = false;

                return;
            }
            
            if (_betSelector.CurrentBet == 0)
            {
                _autoSpinToggle.isOn = false;

                return;
            }

            _exitButton.interactable = false;
            _infoButton.interactable = false;
            _spinButton.interactable = false;
            _betSelector.SetActive(false);
            _freeSpinsButton.SetActive(false);

            StartCoroutine(Spin(false, _betSelector.CurrentBet));
        }

        private IEnumerator Spin(bool isFreeSpins, int bet)
        {
            _isSpining = true;

            _lineDrawer.ClearLines();
            DisableSlotsAnim();

            if (isFreeSpins)
                FreeSpins.Count -= 1;
            else
                Wallet.TryPurchase(bet);

            //Spin anim

            _events.CallPlaySpin();

            bool[] readyList = new bool[_slotLines.Length];

            for (int i = 0; i < _slotLines.Length; i++)
            {
                if (i != 0)
                    yield return new WaitForSeconds(0.05f);

                int currentIndex = i;

                _slotLines[i].StartSpin(() => readyList[currentIndex] = true);
            }

            while (readyList.Where(b => b == false).Count() != 0)
                yield return null;

            Slot[,] grid = GetSlotsGrid();

            List<Slot> winSlots = new List<Slot>();
            List<Line> lines = new List<Line>();
            List<WinningLineInfo> linesInfo = new List<WinningLineInfo>();

            float scale = _calculator.GetBonus(grid, winSlots, lines, linesInfo);

            CallWinLines(linesInfo);
            
            //Results

            foreach (Slot slot in winSlots)
                slot.PlayLineAnim();

            foreach (Line line in lines)
                _lineDrawer.DrawLine(line);

            float winUnrounded = bet * scale;
            int win = (int)winUnrounded;

            if (winUnrounded % 1 > 0)
                win++;

            _winDisplay.SetWin(win);

            if (scale == 0)
            {
                if (winSlots.Count == 0)
                    _events.CallPlayLose();
                else
                    _events.CallPlayWin();

                if (_autoSpinToggle.isOn ||
                    isFreeSpins && _freeSpinsButton.IsPressed)
                    yield return new WaitForSeconds(0.5f);
            }
            else
            {
                _events.CallPlayWin();

                Wallet.AddMoney(win);

                if (_autoSpinToggle.isOn || isFreeSpins && _freeSpinsButton.IsPressed)
                    yield return new WaitForSeconds(1f);
            }

            while (_calculator.IsPlayingAnimation)
                yield return null;

            _betSelector.UpdateBet();
            
            if ((_autoSpinToggle.isOn && _betSelector.CurrentBet != 0) || _freeSpinsButton.IsPressed)
            {
                if (_freeSpinsButton.IsPressed)
                {
                    StartCoroutine(Spin(isFreeSpins, 50));
                }
                else
                {
                    StartCoroutine(Spin(isFreeSpins, _betSelector.CurrentBet));
                }
            }
            else
            {
                if (_betSelector.CurrentBet == 0 && !isFreeSpins)
                    _events.CallEndMoney();

                _autoSpinToggle.isOn = false;

                ActivateButtons();

                _isSpining = false;
            }
        }

        private void CallWinLines(List<WinningLineInfo> infos)
        {
            foreach (var info in infos)
                _events.CallWinningLine(info);
        }
        
        private void ActivateButtons()
        {
            _freeSpinsButton.SetActive(true);
            _betSelector.SetActive(true);
            
            _exitButton.interactable = true;
            _infoButton.interactable = true;
            _spinButton.interactable = true;
            _autoSpinToggle.interactable = true;
        }

        private void DisableSlotsAnim()
        {
            int width = _slotLines.Count();
            int height = _slotLines.First().TargetSlots.Count();

            for (int x = 0; x < width; x++)
            {
                Slot[] line = _slotLines[x].TargetSlots;

                for (int y = 0; y < height; y++)
                    line[y].HideLineAnim();
            }
        }

        private Slot[,] GetSlotsGrid()
        {
            int width = _slotLines.Count();
            int height = _slotLines.First().TargetSlots.Count();

            Slot[,] grid = new Slot[width, height];

            for (int x = 0; x < width; x++)
            {
                Slot[] line = _slotLines[x].TargetSlots;

                for (int y = 0; y < height; y++)
                {
                    grid[x, y] = line[y];
                }
            }

            return grid;
        }
    }
}