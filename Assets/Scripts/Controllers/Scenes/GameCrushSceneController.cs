using System.Collections;
using Models.Scenes;
using Slots.Game.Values;
using Types;
using UnityEngine;
using UnityEngine.UI;
using Views.Game;
using Views.General;
using Views.GoldMineRush;
using Views.RoyalRoulette;

namespace Controllers.Scenes
{
    public class GameCrushSceneController : AbstractSceneController
    {
        [Space(5)][Header("Views")]
        [SerializeField] private BetSelectorView _betSelectorView;
        [SerializeField] private TotalBetView _totalBetView;
        [SerializeField] private TotalBetView _winView;
        [SerializeField] private PersonView _personView;

        [Space(1)] [Header("Panels")] 
        [SerializeField] private GoldResultPanel _losePanel;
        [SerializeField] private PanelView _confirmationPanel;
        [SerializeField] private NotEnoughCoinsPanelView _notEnoughCoinsPanelView;

        [Space(1)] [Header("Buttons")]
        [SerializeField] private Button _startBtn;
        [SerializeField] private Button _homeBtn;
        [SerializeField] private Button _cashOutBtn;

        private GoldMineRushSceneModel _model;
        private Coroutine _timerCoroutine;
        
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
            _model = new GoldMineRushSceneModel();
        }

        protected override void Subscribe()
        {
            _startBtn.onClick.AddListener(OnPressStartBtn);
            _cashOutBtn.onClick.AddListener(OnPressCashOutBtn);
            _homeBtn.onClick.AddListener(OnPressHomeBtn);
        }

        protected override void Unsubscribe()
        {
            _startBtn.onClick.RemoveAllListeners();
            _cashOutBtn.onClick.RemoveAllListeners();
            _homeBtn.onClick.RemoveAllListeners();
        }

        private void SetState(GoldMineCrashState state)
        {
            switch (state)
            {
                case GoldMineCrashState.StartState:
                    SetStartState();
                    break;
                case GoldMineCrashState.NoMoneyState:
                    SetNoMoneyState();
                    break;
                case GoldMineCrashState.SpinningState:
                    SetSpinningState();
                    break;
            }
        }

        private void SetStartState()
        {
            _totalBetView.UpdateMoney(0);
            
            _startBtn.gameObject.SetActive(true);
            _startBtn.interactable = true;
            
            _cashOutBtn.gameObject.SetActive(false);
            _betSelectorView.SetActive(true);
            _homeBtn.interactable = true;
            
            _personView.SetState(GoldMineCrashState.StartState);
        }

        private void SetNoMoneyState()
        {
            _totalBetView.UpdateMoney(0);
            
            _startBtn.gameObject.SetActive(true);
            _startBtn.interactable = false;
            _cashOutBtn.gameObject.SetActive(false);
            _betSelectorView.SetActive(false);
            _homeBtn.interactable = true;
            
            _personView.SetState(GoldMineCrashState.NoMoneyState);
            
            OpenEndMoneyPanel();
        }

        private void SetSpinningState()
        {
            _winView.UpdateMoney(0);
            //_totalBetView.UpdateMoney(0);
            
            _startBtn.gameObject.SetActive(false);
            _cashOutBtn.gameObject.SetActive(true);
            _betSelectorView.SetActive(false);
            _homeBtn.interactable = false;
            
            _personView.SetState(GoldMineCrashState.SpinningState);
        }

        private void CheckStartState()
        {
            SetState(_model.GetStartState());
        }

        private void OnPressStartBtn()
        {
            base.SetClickClip();
            
            int value = _betSelectorView.CurrentBet;

            Wallet.TryPurchase(value);
            _totalBetView.UpdateMoney(value);
            
            SetState(GoldMineCrashState.SpinningState);
            
            StartTimer();
        }

        private void OnPressCashOutBtn()
        {
            base.PlaySound(MusicType.WinClip);
            
            _personView.SetState(GoldMineCrashState.StartState);
            CheckStartState();
            
            SetWinResult(_personView.CurrenX);
            
            StopCoroutine(_timerCoroutine);
        }

        private void OnPressHomeBtn()
        {
            base.SetClickClip();
            
            OpenConfirmationPanel();
        }

        private void OnResultEndAnim()
        {
            _losePanel.OnEndAnimAction -= OnResultEndAnim;
            CheckStartState();
        }

        private void StartTimer()
        {
            _timerCoroutine = StartCoroutine(StartTimerCoroutine());
        }

        private void SetWinResult(float multiplayer)
        {
            Debug.Log($"Bet {_betSelectorView.CurrentBet} Multiplayer {multiplayer}");
            
            _winView.UpdateMoney((int)_model.GetReward(_betSelectorView.CurrentBet, multiplayer));
        }

        private void ShowLosePanel()
        {
            base.PlaySound(MusicType.LoseClip);
            
            _losePanel.OnEndAnimAction += OnResultEndAnim;
            _losePanel.ShowResult();
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
                    base.SetClickClip();
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

        private IEnumerator StartTimerCoroutine()
        {
            var sec = _model.GetCrashValue();

            yield return new WaitForSeconds(sec);
            
            _personView.SetState(GoldMineCrashState.StartState);
            ShowLosePanel();
        }
    }
}