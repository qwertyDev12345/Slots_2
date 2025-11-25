using UnityEngine;
using UnityEngine.UI;

namespace Views.RoyalRoulette
{
    public class CoinView : MonoBehaviour
    {
        [SerializeField] private Text _text;

        public void SetCount(int value)
        {
            _text.text = value.ToString();
        }
    }
}