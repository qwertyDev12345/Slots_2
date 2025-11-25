using Slots.Game.Machine;
using UnityEngine;
using UnityEngine.Events;

namespace Slots.Game
{
    public class SlotMachineEvents : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onClickExit = null;
        [SerializeField] private UnityEvent _onEndMoney = null;
        
        [SerializeField] private UnityEvent<WinningLineInfo> _onWinningLine = null;

        [SerializeField] private UnityEvent _playClick = null;
        [SerializeField] private UnityEvent _playSpin = null;
        [SerializeField] private UnityEvent _playWin = null;
        [SerializeField] private UnityEvent _playLose = null;

        public UnityEvent OnClickExit => _onClickExit;
        public UnityEvent OnEndMoney => _onEndMoney;
        
        public UnityEvent PlayClick => _playClick;
        public UnityEvent PlaySpin => _playSpin;
        public UnityEvent PlayWin => _playWin;
        public UnityEvent PlayLose => _playLose;
        
        public void CallExit() =>
            _onClickExit?.Invoke();

        public void CallEndMoney() =>
            _onEndMoney?.Invoke();

        public void CallWinningLine(WinningLineInfo info) =>
            _onWinningLine?.Invoke(info);

        public void CallPlayClick() =>
            _playClick?.Invoke();

        public void CallPlaySpin() =>
            _playSpin?.Invoke();

        public void CallPlayWin() =>
            _playWin?.Invoke();

        public void CallPlayLose() =>
            _playLose?.Invoke();
    }
}