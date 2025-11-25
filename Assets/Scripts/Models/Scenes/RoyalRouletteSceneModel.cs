using System.Collections.Generic;
using System.Linq;
using Data;
using Slots.Game.Values;
using Types;
using Views.RoyalRoulette;

namespace Models.Scenes
{
    public class RoyalRouletteSceneModel
    {
        private List<BetBtnView> _selectedBetBtns;
        private List<int> _redNumbers;
        private List<PlayerBet> _bets;
        private int _totalBet;
        
        public BetBtnView SelectedBetBtn { get; private set; }
        public int TotalBet => _totalBet;

        public RoyalRouletteSceneModel(List<int> redNumbers)
        {
            _redNumbers = new List<int>(redNumbers);
            _bets = new List<PlayerBet>();
            _totalBet = 0;
        }

        public void Reset()
        {
            _totalBet = 0;

            if (_selectedBetBtns is { Count: > 0 })
            {
                _selectedBetBtns.Clear();
            }

            SelectedBetBtn = null;
        }

        public bool TryPurchaseMoney(BetBtnView view)
        {
            return SelectedBetBtn != null && SelectedBetBtn != view;
        }

        public bool TryAddMoney(int value)
        {
            if (value > 0)
            {
                Wallet.AddMoney(value);
                return true;
            }

            return false;
        }
        
        public bool IsRed(int n)   => n != 0 && _redNumbers.Contains(n);

        public void AddSelectedBetBtn(BetBtnView view)
        {
            _selectedBetBtns ??= new List<BetBtnView>();

            SelectedBetBtn = view;
            _selectedBetBtns.Add(view);
        }

        public void PlaceBet(int amount)
        {
            _bets.Add(new PlayerBet
            {
                Type = SelectedBetBtn.Type,
                Value = SelectedBetBtn.Value,
                Amount = amount
            });

            _totalBet += amount;
        }

        public List<BetResult> EvaluateRound(int resultNumber)
        {
            var results = new List<BetResult>();

            foreach (var bet in _bets)
            {
                bool win = false;
                int odds = 0;

                switch (bet.Type)
                {
                    case BetBtnType.Straight:
                        win = resultNumber == bet.Value;
                        odds = 35;
                        break;

                    case BetBtnType.Zero:
                        win = resultNumber == 0;
                        odds = 35;
                        break;

                    case BetBtnType.Dozen:
                        win = resultNumber >= (bet.Value - 1) * 12 + 1 && resultNumber <= bet.Value * 12;
                        odds = 2;
                        break;

                    case BetBtnType.Column:
                        win = IsInColumn(resultNumber, bet.Value);
                        odds = 2;
                        break;

                    case BetBtnType.Color:
                        if (resultNumber != 0)
                        {
                            bool isRed = _redNumbers.Contains(resultNumber);
                            win = bet.Value == 0 ? isRed : !isRed;
                        }

                        odds = 1;
                        break;

                    case BetBtnType.Parity:
                        if (resultNumber != 0)
                        {
                            bool isEven = resultNumber % 2 == 0;
                            win = bet.Value == 0 ? isEven : !isEven;
                        }

                        odds = 1;
                        break;

                    case BetBtnType.Range:
                        if (resultNumber != 0)
                        {
                            win = bet.Value == 0
                                ? resultNumber >= 1 && resultNumber <= 18
                                : resultNumber >= 19 && resultNumber <= 36;
                        }

                        odds = 1;
                        break;
                }

                int profit = win ? bet.Amount * odds : 0;
                int payout = win ? bet.Amount + profit : 0;

                results.Add(new BetResult
                {
                    Type = bet.Type,
                    Value = bet.Value,
                    Amount = bet.Amount,
                    IsWin = win,
                    Profit = profit,
                    Payout = payout
                });
            }

            _bets.Clear();
            return results;
        }

        public int GetTotalPayout(IEnumerable<BetResult> results)
        {
            return results.Sum(r => r.Payout);
        }

        public RoyalRouletteSceneState GetStartState()
        {
            return Wallet.Money > 0 ? RoyalRouletteSceneState.Start : RoyalRouletteSceneState.NoMoney;
        }

        private bool IsInColumn(int number, int columnIndex)
        {
            if (number == 0) return false;

            return columnIndex switch
            {
                1 => number % 3 == 1,
                2 => number % 3 == 2,
                3 => number % 3 == 0,
                _ => false
            };
        }
    }
}