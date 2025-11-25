using UnityEngine;
using UnityEngine.UI;

namespace Slots.Game.Audio
{
    [RequireComponent(typeof(Toggle))]
    public class PlayOnToggleClick : MonoBehaviour
    {
        private void Awake()
        {
            SlotMachineEvents events = GetComponentInParent<SlotMachineEvents>();

            GetComponent<Toggle>().onValueChanged.AddListener((active) =>
                events.CallPlayClick());
        }
    }
}