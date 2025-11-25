using UnityEngine;
using UnityEngine.UI;

using Views.Settings;
using Sounds;
using Types;
using Views.General;

namespace Controllers.Scenes
{
    public class SettingsSceneController : AbstractSceneController
    {
        [Space(5)] [Header("Views")]
        [SerializeField] private PanelView _privacyPanel;
        
        [Space(5)][Header("Buttons")]
        [SerializeField] 
        private SwitcherBtn _soundSwitcherView;
        [SerializeField] 
        private SwitcherBtn _musicSwitcherView;
        [SerializeField] 
        private Button _ppBtn;
        [SerializeField] 
        private Button _backBtn;
        
        protected override void OnSceneEnable()
        {
            
        }

        protected override void OnSceneStart()
        {
            UpdateSoundBtnSprite();
            UpdateMusicBtnSprite();
            base.PlayMusic();
        }

        protected override void OnSceneDisable()
        {
            
        }

        protected override void Subscribe()
        {
            _ppBtn.onClick.AddListener(OnPressPrivacyBtn);
            //_backBtn.onClick.AddListener(delegate { LoadScene(MusicType.MenuClip.ToString()); });

            _soundSwitcherView.PressBtnAction += ChangeSoundState;
            _musicSwitcherView.PressBtnAction += ChangeMusicState;

        }

        protected override void Initialize()
        {
            
        }

        protected override void Unsubscribe()
        {
            _ppBtn.onClick.RemoveAllListeners();
            _backBtn.onClick.RemoveAllListeners();
            
            _soundSwitcherView.PressBtnAction += ChangeSoundState;
            _musicSwitcherView.PressBtnAction += ChangeMusicState;
        }

        private void UpdateSoundBtnSprite()
        {
            _soundSwitcherView.SetSprite(SoundsStates.CanPlaySound ? 0 : 1);
        }

        private void UpdateMusicBtnSprite()
        {
            _musicSwitcherView.SetSprite(SoundsStates.CanPlayMusic ? 0: 1);
        }

        private void ChangeSoundState()
        {
            SoundsStates.ChangeSoundsState();
            
            base.SetClickClip();
            
            UpdateSoundBtnSprite();
        }

        private void ChangeMusicState()
        {
            base.SetClickClip();

            SoundsStates.ChangeMusicState();
            
            base.PlayMusic();
            
            UpdateMusicBtnSprite();
        }

        private void OnPressPrivacyBtn()
        {
            base.SetClickClip();
            
            OpenPrivacyPanel();
        }

        private void OpenPrivacyPanel()
        {
            _privacyPanel.PressBtnAction += OnReceiveAnswerPrivacyPanel;
            _privacyPanel.gameObject.SetActive(true);
        }

        private void OnReceiveAnswerPrivacyPanel(int answer)
        {
            base.SetClickClip();
            
            _privacyPanel.PressBtnAction -= OnReceiveAnswerPrivacyPanel;
            _privacyPanel.gameObject.SetActive(false);
        }
    }
}