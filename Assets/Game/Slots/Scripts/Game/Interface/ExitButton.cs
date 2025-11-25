using UnityEngine;
using UnityEngine.UI;

namespace Slots.Game.Interface
{
    [RequireComponent(typeof(Button))]
    public class ExitButton : MonoBehaviour
    {
        private void Awake()
        {
            SlotMachineEvents events = GetComponentInParent<SlotMachineEvents>();
            
            GetComponent<Button>().onClick.AddListener(() => 
                events.CallExit());
        }
    }
}