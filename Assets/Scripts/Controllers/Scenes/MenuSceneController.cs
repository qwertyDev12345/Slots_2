using Types;
using UnityEngine;
using UnityEngine.UI;
using Views.Menu;

namespace Controllers.Scenes
{
    public class MenuSceneController : AbstractSceneController
    {
        [SerializeField] private Button _shopBtn;
        [SerializeField] private Button _slot1Btn;
        [SerializeField] private Button _slot2Btn;
        [SerializeField] private Button _rouletteBtn;
        [SerializeField] private Button _crashGame1Btn;
        [SerializeField] private Button _crashGame2Btn;
        [SerializeField] private ShopPanel _shopPanel;
        
        
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
            _shopBtn.onClick.AddListener(OnPressShopBtn);
            _slot1Btn.onClick.AddListener(OnPressSlot1Btn);
            _slot2Btn.onClick.AddListener(OnPressSlo2Btn);
            _rouletteBtn.onClick.AddListener(OnPressRouletteBtn);
            _crashGame1Btn.onClick.AddListener(OnPressCrashGame1Btn);
            _crashGame2Btn.onClick.AddListener(OnPressCrashGame2Btn);
        }

        protected override void Unsubscribe()
        {
            _shopBtn.onClick.RemoveAllListeners();
            _slot1Btn.onClick.RemoveAllListeners();
            _slot2Btn.onClick.RemoveAllListeners();
            _rouletteBtn.onClick.RemoveAllListeners();
            _crashGame1Btn.onClick.RemoveAllListeners();
            _crashGame2Btn.onClick.RemoveAllListeners();
        }

        private void OnPressShopBtn()
        {
            base.SetClickClip();
            
            OpenShopPanel();
        }

        private void OnPressSlot1Btn()
        {
            OpenScene(SceneType.CrystalStormScene);
        }

        private void OnPressSlo2Btn()
        {
            OpenScene(SceneType.TurboSlotScene);
        }

        private void OnPressRouletteBtn()
        {
            OpenScene(SceneType.StelarWheelScene);
        }

        private void OnPressCrashGame1Btn()
        {
            OpenScene(SceneType.DipeDiveScene);
        }

        private void OnPressCrashGame2Btn()
        {
            OpenScene(SceneType.ReactorRunScene);
        }
        
        private void OpenShopPanel()
        {
            _shopPanel.PressBtnAction += OnReceiveAnswerShopPanel;
            _shopPanel.Show();
        }

        private void OpenScene(SceneType type)
        {
            base.LoadScene(type);
        }

        private void OnReceiveAnswerShopPanel(int answer)
        {
            base.SetClickClip();
            
            _shopPanel.PressBtnAction -= OnReceiveAnswerShopPanel;
            _shopPanel.Hide();
        }
    }
}