using Controllers.Slots;
using Slots.Game.Values;
using Types;
using UnityEngine;
using Views.Game;
using Views.General;

namespace Controllers.Scenes
{
    public class SlotsScenesController : AbstractSceneController
    {
        [SerializeField] private ActionController _actionController;
        [SerializeField] private PanelView _confirmationPanel;
        [SerializeField] private NotEnoughCoinsPanelView _notEnoughCoinsPanelView;
        
        protected override void OnSceneEnable()
        {
            
        }

        protected override void OnSceneStart()
        {
            
        }

        protected override void OnSceneDisable()
        {
            
        }

        protected override void Initialize()
        {
            
        }

        protected override void Subscribe()
        {
            _actionController.OnPressHomeBtnAction += OnPressHomeBtn;
            _actionController.OnEndMoneyAction += OnEndMoney;
            _actionController.OnSpinAction += PlaySpinSound;
            _actionController.OnWinLineAction += PlayWinSound;
            _actionController.OnPressBtnAction += PlayClickSound;
        }

        protected override void Unsubscribe()
        {
            _actionController.OnPressHomeBtnAction -= OnPressHomeBtn;
            _actionController.OnEndMoneyAction -= OnEndMoney; 
            _actionController.OnSpinAction -= PlaySpinSound;
            _actionController.OnWinLineAction -= PlayWinSound;
            _actionController.OnPressBtnAction -= PlayClickSound;
        }

        private void OnPressHomeBtn()
        {
            OpenConfirmationPanel();
        }
        
        private void OnEndMoney()
        {
            OpenEndMoneyPanel();
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
                    break;
            }
        }

        private void PlaySpinSound()
        {
            base.PlaySound(MusicType.SpinClip);
        }

        private void PlayClickSound()
        {
            base.SetClickClip();
        }

        private void PlayWinSound()
        {
            base.PlaySound(MusicType.WinClip);
        }
    }
}