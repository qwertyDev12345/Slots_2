using System.Collections.Generic;

namespace Slots.Game.Machine
{
    public class Line
    {
        public List<Slot> Slots = new List<Slot>();

        public Line(List<Slot> slots) =>
            Slots = slots;
    }
}