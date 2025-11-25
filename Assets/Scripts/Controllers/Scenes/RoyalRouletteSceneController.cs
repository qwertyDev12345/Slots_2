using System;
using System.Collections.Generic;
using Data;
using Models.Scenes;
using Slots.Game.Values;
using UnityEngine;
using UnityEngine.UI;

using Views.Game;
using Views.RoyalRoulette;

using SO;
using Types;
using Views.General;

namespace Controllers.Scenes
{
    public class RoyalRouletteSceneController : AbstractSceneController
    {
        [Space(5), Header("Scene config")] [SerializeField]
        private RouletteConfig _config;

        [Space(1), Header("Buttons")] 
        [SerializeField] private List<BetBtnView> _betBtnViews;
        [SerializeField] private BetSelectorView _betSelectorView;
        [SerializeField] private Button _spinBtn;
        [SerializeField] private Button _homeBtn;

        [Space(1), Header("Prefabs")] [SerializeField]
        private GameObject _coinViewPrefab;

        [Space(1), Header("Views")] 
        [SerializeField] private RouletteBall _rouletteBall;
        [SerializeField] private TotalBetView _totalBetView;
        [SerializeField] private TotalBetView _winView;
        [SerializeField] private ResultPanel _resultPanel;
        [SerializeField] private PanelView _confirmationPanel;
        [SerializeField] private NotEnoughCoinsPanelView _notEnoughCoinsPanelView;

        private RoyalRouletteSceneModel _model;
        private Action _onEndAnimHandler;
        
        protected override void OnSceneEnable()
        {
            CheckStartState();
        }

        protected override void OnSceneStart()
        {
            
        }

        protected override void OnSceneDisable()
        {
            
        }

        protected override void Initialize()
        {
            _model = new RoyalRouletteSceneModel(_config.redNumbers);
        }

        protected override void Subscribe()
        {
            _betBtnViews.ForEach(btn => btn.OnPressBtn += OnPressBetBtn);
            _betSelectorView.OnValueChangedAction += OnBetSelectorChanged;
            _spinBtn.onClick.AddListener(OnPressSpinBtn);
            _rouletteBall.OnEndSpinAction += OnEndBallSpin;
            
            _homeBtn.onClick.AddListener(OpenConfirmationPanel);
        }

        protected override void Unsubscribe()
        {
            _betBtnViews.ForEach(btn => btn.OnPressBtn -= OnPressBetBtn);
            _betSelectorView.OnValueChangedAction -= OnBetSelectorChanged;
            _spinBtn.onClick.RemoveAllListeners();
            _rouletteBall.OnEndSpinAction -= OnEndBallSpin;
            
            _homeBtn.onClick.RemoveAllListeners();
        }

        private void SetState(RoyalRouletteSceneState state)
        {
            switch (state)
            {
                case RoyalRouletteSceneState.Start:
                    SetStartState();
                    break;
                case RoyalRouletteSceneState.Spinning:
                    SetSpinningState();
                    break;
                case RoyalRouletteSceneState.PressedBet:
                    SetStatePressedBet();
                    break;
                case RoyalRouletteSceneState.NoMoney:
                    SetNoMoneyState();
                    break;
            }
        }

        private void SetStartState()
        {
            _spinBtn.interactable = false;
            _betSelectorView.SetActive(false);
            _betBtnViews.ForEach(btn => btn.SetState(true));
            _homeBtn.interactable = true;
        }

        private void SetStatePressedBet()
        {
            _betSelectorView.SetActive(true);
            _spinBtn.interactable = true;
            _homeBtn.interactable = true;
        }

        private void SetSpinningState()
        {
            _spinBtn.interactable = false;
            _betSelectorView.SetActive(false);
            _betBtnViews.ForEach(btn => btn.SetState(false));
            _homeBtn.interactable = false;
        }

        private void SetNoMoneyState()
        {
            _spinBtn.interactable = false;
            _betSelectorView.SetActive(false);
            _betBtnViews.ForEach(btn => btn.SetState(false));
            _homeBtn.interactable = true;
            
            OpenEndMoneyPanel();
        }

        private void OnPressBetBtn(BetBtnView view)
        {
            base.SetClickClip();
            
            SetStatePressedBet();

            if (_model.TryPurchaseMoney(view))
            {
                PurchaseMoney();
            }

            view.SetState(false);
            
            _model.AddSelectedBetBtn(view);

            GameObject coinGo = Instantiate(_coinViewPrefab, view.gameObject.transform);
            CoinView coinView = coinGo.GetComponent<CoinView>();
            
            view.SetCoin(coinView);
            
            coinView.SetCount(_betSelectorView.CurrentBet);
        }

        private void OnBetSelectorChanged(int value)
        {
            base.SetClickClip();
            
            _model.SelectedBetBtn.ChangeBetOnCoin(value);
        }

        private void OnPressSpinBtn()
        {
            base.PlaySound(MusicType.SpinRoulette);
            
            _winView.UpdateMoney(0);
            PurchaseMoney();
            SetState(RoyalRouletteSceneState.Spinning);
            
            _rouletteBall.SpinRandom();
        }

        private void PurchaseMoney()
        {
            int value = _betSelectorView.CurrentBet;
            
            Wallet.TryPurchase(value);
            
            _model.PlaceBet(value);
            
            _totalBetView.UpdateMoney(_model.TotalBet);
        }

        private void OnEndBallSpin(int number)
        {
            List<BetResult> results = _model.EvaluateRound(number);
            var total = _model.GetTotalPayout(results);

            bool isRed = _model.IsRed(number);

            _resultPanel.ShowResult(number, isRed);
            
            _onEndAnimHandler = () => OnEndResultAnim(total);
            _resultPanel.OnEndAnimAction += _onEndAnimHandler;
        }

        private void OnEndResultAnim(int total)
        {
            if (_onEndAnimHandler != null)
            {
                _resultPanel.OnEndAnimAction -= _onEndAnimHandler;
                _onEndAnimHandler = null;
            }

            SetResult(total);
        }

        private void SetResult(int total)
        {
            if (_model.TryAddMoney(total))
            {
                _winView.UpdateMoney(total);
                base.PlaySound(MusicType.WinClip);
            }
            else
            {
                base.PlaySound(MusicType.LoseClip);
            }
            
            CheckStartState();
        }

        private void CheckStartState()
        {
            _model.Reset();
            _totalBetView.UpdateMoney(_model.TotalBet);
            
            SetState(_model.GetStartState());
        }
        
        private void OpenEndMoneyPanel()
        {
            _notEnoughCoinsPanelView.Show();
            _notEnoughCoinsPanelView.PressBtnAction += OnReceiveAnswerEndMoneyPanel;
        }

        private void OpenConfirmationPanel()
        {
            _confirmationPanel.Show();
            _confirmationPanel.PressBtnAction += OnReceiveAnswerConfirmPanel;
        }

        private void OnReceiveAnswerConfirmPanel(int answer)
        {
            _confirmationPanel.PressBtnAction -= OnReceiveAnswerConfirmPanel;
            
            switch (answer)
            {
                case 0:
                    base.LoadScene(SceneType.MenuScene);
                    break;
                case 1:
                    _confirmationPanel.Hide();
                    break;
            }
        }

        private void OnReceiveAnswerEndMoneyPanel(int answer)
        {
            _notEnoughCoinsPanelView.PressBtnAction -= OnReceiveAnswerEndMoneyPanel;
            _notEnoughCoinsPanelView.Hide();

            switch (answer)
            {
                case 0:
                    base.LoadScene(SceneType.MenuScene);
                    break;
                case 1:
                    Wallet.AddMoney(100);
                    CheckStartState();
                    break;
            }
        }
    }
}