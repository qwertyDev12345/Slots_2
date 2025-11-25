using UnityEngine;
using UnityEngine.UI;

namespace Slots.Game.Audio
{
    [RequireComponent(typeof(Button))]
    public class PlayOnClick : MonoBehaviour
    {
        private void Awake()
        {
            SlotMachineEvents events = GetComponentInParent<SlotMachineEvents>();

            GetComponent<Button>().onClick.AddListener(() =>
                events.CallPlayClick());
        }
    }
}