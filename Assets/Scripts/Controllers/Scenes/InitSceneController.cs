using Types;
#if UNITY_IOS
    using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;
using UnityEngine.UI;
using Views.General;
using Views.Init;

namespace Controllers.Scenes
{
    public class InitSceneController : AbstractSceneController
    {
        [SerializeField] private PanelView _privacyPanelView;
        [SerializeField] private PanelView _termsPanelView;
        [SerializeField] private LinkHandler _linkHandler;
        [SerializeField] private Button _startBtn;
        
        protected override void OnSceneEnable()
        {
            CheckATT();
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
            _startBtn.onClick.AddListener(OnPressStartBtn);
            _linkHandler.OnPressTextBtnAction += OnPressTextBtn;
        }

        protected override void Unsubscribe()
        {
            _startBtn.onClick.RemoveAllListeners();
            _linkHandler.OnPressTextBtnAction -= OnPressTextBtn;
        }

        private void CheckATT()
        {
#if UNITY_IOS
            var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            
            if (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking();
            }
#endif
        }

        private void OnPressStartBtn()
        {
            LoadMenuScene();
        }

        private void LoadMenuScene()
        {
            base.LoadScene(SceneType.MenuScene);
        }

        private void OnPressTextBtn(int value)
        {
            base.SetClickClip();
            
            if (value == 0)
            {
                OpenPrivacyBtn();
            }
            else
            {
                OpenTermsBtn();
            }
        }

        private void OpenPrivacyBtn()
        {
            _privacyPanelView.PressBtnAction += OnReceiveAnswerPrivacyPanel;
            _privacyPanelView.Show();
        }

        private void OnReceiveAnswerPrivacyPanel(int answer)
        {
            base.SetClickClip();
            
            _privacyPanelView.PressBtnAction -= OnReceiveAnswerPrivacyPanel;
            _privacyPanelView.Hide();
        }
        
        private void OpenTermsBtn()
        {
            _termsPanelView.PressBtnAction += OnReceiveAnswerTermsPanel;
            _termsPanelView.Show();
        }

        private void OnReceiveAnswerTermsPanel(int answer)
        {
            base.SetClickClip();
            
            _termsPanelView.PressBtnAction -= OnReceiveAnswerTermsPanel;
            _termsPanelView.Hide();
        }
    }
}