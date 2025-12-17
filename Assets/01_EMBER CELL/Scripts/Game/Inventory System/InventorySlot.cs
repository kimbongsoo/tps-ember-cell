using System;

namespace TEC
{
    [Serializable]
    public class InventorySlot
    {
        public string guid;
        public int amount;

        public InventorySlot(string guid, int amount)
        {
            this.guid = guid;
            this.amount = amount;
        }
    }
}
