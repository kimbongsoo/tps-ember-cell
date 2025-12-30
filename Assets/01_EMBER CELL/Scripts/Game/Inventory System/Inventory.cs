using UnityEngine;

namespace TEC
{
    public class Inventory : SingletonBase<Inventory>
    {
        public int SlotCount
        {
            get => slotCount;
            set
            {
                int clamped = Mathf.Max(0, value);
                if (slotCount == clamped)
                    return;

                slotCount = clamped;
                OnSlotCountChanged?.Invoke(slotCount);
            }
        }

        public System.Action<int> OnSlotCountChanged;

        [Header("Inventory Setting")]
        [SerializeField] private int defaultSlotCount = 24;

        private int slotCount = 0;

        private void Start()
        {
            SlotCount = defaultSlotCount;
        }

        public void AddSlot(int amount = 1)
        {
            if (amount <= 0)
                return;

            SlotCount += amount;
        }
    }
}
