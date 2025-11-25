using UnityEngine;
using UnityEngine.UI;

namespace Slots.Game.InfoPanel
{
    public class InfoLine : MonoBehaviour
    {
        [SerializeField] private Text _count = null;
        [SerializeField] private Text _prize = null;

        public void SetInfo(int count, float prize)
        {
            _count.text = count.ToString();
            _prize.text = "x" + prize;
        }
    }
}