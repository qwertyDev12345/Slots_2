using System;
using System.Collections.Generic;
using System.Linq;
using Slots.Data.Slots;
using Slots.Game.Values;
using UnityEngine;
using DG.Tweening;
using Slots.Game.Interface;
using UnityEngine.UI;

namespace Slots.Game.Machine
{
    public class BonusCalculator : MonoBehaviour
    {
        [SerializeField] private Image _2XTimer = null;
        [SerializeField] private MultiplierAnimation _multiplierAnimation = null;
        [SerializeField] private FreeSpinsAnimation _freeSpinsAnimation = null;

        private float _multiplierTimer = 0f;

        private bool IsActiveMultiplier => _multiplierTimer > 0f;

        private FieldGenerator _generator = null;

        public bool IsPlayingAnimation => _multiplierAnimation.IsPlaying || _freeSpinsAnimation.IsPlaying;
        
        private void Awake()
        {
            _generator = GetComponentInParent<FieldGenerator>();

            _2XTimer.transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            if (_multiplierTimer > 0)
            {
                _multiplierTimer -= Time.deltaTime / 10f;

                _2XTimer.fillAmount = _multiplierTimer;

                if (_multiplierTimer <= 0)
                {
                    _2XTimer.DOKill();
                    _2XTimer.transform.DOScale(0f, 0.25f).Play();
                }
            }
        }

        public float GetBonus(Slot[,] grid, List<Slot> winSlots, List<Line> lines, List<WinningLineInfo> linesInfo)
        {
            float scale = 0f;

            CheckScatter(grid, winSlots, linesInfo);
            CheckMultiplier(grid, winSlots, linesInfo);

            scale = CheckCombinations(grid, winSlots, lines, linesInfo);

            if (IsActiveMultiplier)
                scale *= 2f;

            return scale;
        }

        private void CheckScatter(Slot[,] grid, List<Slot> winSlots, List<WinningLineInfo> linesInfo)
        {
            List<Slot> slots = new List<Slot>();

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y].SlotData.Type == SlotType.Scatter)
                        slots.Add(grid[x, y]);
                }
            }

            if (slots.Count >= 3)
            {
                linesInfo.Add(new WinningLineInfo()
                {
                    slotName = "Scatter",
                    countInLine = slots.Count
                });
                
                winSlots.AddRange(slots);

                int prizeIndex = Mathf.Clamp(slots.Count, 0, 5);

                int prize = (int)slots[0].SlotData.Bonuses[prizeIndex];
                
                FreeSpins.Count += prize;
                
                _freeSpinsAnimation.ShowAnimation(prize);
            }
        }

        private void CheckMultiplier(Slot[,] grid, List<Slot> winSlots, List<WinningLineInfo> linesInfo)
        {
            List<Slot> slots = new List<Slot>();

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y].SlotData.Type == SlotType.Multiplier)
                        slots.Add(grid[x, y]);
                }
            }

            if (slots.Count >= 2)
            {
                linesInfo.Add(new WinningLineInfo()
                {
                    slotName = "Multiplier",
                    countInLine = slots.Count
                });

                winSlots.AddRange(slots);

                Action makeMultiplier = () =>
                {
                    _multiplierTimer = 1f;

                    _2XTimer.DOKill();
                    _2XTimer.transform.DOScale(1f, 0.25f).Play();
                
                    _multiplierAnimation.ShowAnimation();
                };

                if (_freeSpinsAnimation.IsPlaying)
                    _freeSpinsAnimation.OnEndedAnimation += makeMultiplier;
                else
                    makeMultiplier?.Invoke();
            }
        }

        private float CheckCombinations(Slot[,] grid, List<Slot> winSlots, List<Line> lines, List<WinningLineInfo> linesInfo)
        {
            float scale = 0f;

            List<bool[,]> combinations = _generator.Combinations;

            for (int i = 0; i < combinations.Count; i++)
            {
                float combinationScale = CheckCombination(grid, combinations[i], winSlots, lines, linesInfo);

                if (combinationScale != 0)
                {
                    if (scale == 0)
                        scale = combinationScale;
                    else
                        scale *= combinationScale;
                }
            }

            return scale;
        }

        private float CheckCombination(Slot[,] grid, bool[,] combination, List<Slot> winSlots, List<Line> lines, List<WinningLineInfo> linesInfo)
        {
            List<Slot> line = new List<Slot>();
            List<Slot> fullLine = new List<Slot>();

            Slot targetSlot = null;

            bool isBreaked = false;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (combination[x, y] == true)
                    {
                        Slot slot = grid[x, y];

                        fullLine.Add(slot);

                        if (isBreaked == false)
                        {
                            if (slot.SlotData.Type == SlotType.Scatter || slot.SlotData.Type == SlotType.Multiplier)
                            {
                                isBreaked = true;

                                continue;
                            }

                            if (slot.SlotData.Type == SlotType.Wild)
                                line.Add(slot);
                            else
                            {
                                if (targetSlot != null)
                                {
                                    if (slot.SlotData != targetSlot.SlotData)
                                    {
                                        isBreaked = true;

                                        continue;
                                    }

                                    line.Add(slot);
                                }
                                else
                                {
                                    targetSlot = slot;

                                    line.Add(slot);
                                }
                            }
                        }
                    }
                }
            }

            if (targetSlot != null && targetSlot.SlotData.Bonuses.ContainsKey(line.Count))
            {
                List<SlotData> slots = SlotsRandomizer.GetPool()
                    .Where(s => s.Type == SlotType.Standart)
                    .ToList();
                
                int index = slots.IndexOf(targetSlot.SlotData);
                
                index++;

                linesInfo.Add(new WinningLineInfo()
                {
                    slotName = "Slot " + index,
                    countInLine = line.Count
                });

                foreach (Slot slot in line)
                {
                    if (!winSlots.Contains(slot))
                        winSlots.Add(slot);
                }

                lines.Add(new Line(fullLine));

                return targetSlot.SlotData.Bonuses[line.Count];
            }

            return 0f;
        }
    }
}