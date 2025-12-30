using UnityEngine;

namespace TEC
{
    public class InventoryUI : UIBase
    {
        public static InventoryUI Instance => UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);

        [Header("UI References")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Transform slotHolder;

        private Slot[] slots = null;
        private bool isActiveInventory = false;

        private void Awake()
        {
            if (slotHolder != null)
                slots = slotHolder.GetComponentsInChildren<Slot>(true);

            if (inventoryPanel != null)
                inventoryPanel.SetActive(isActiveInventory);
        }

        private void OnEnable()
        {
            Inventory.Singleton.OnSlotCountChanged += OnSlotCountChanged;
            InputManager.Singleton.OnInventory += ToggleInventory;

            OnSlotCountChanged(Inventory.Singleton.SlotCount);
        }

        private void OnDisable()
        {
            if (Inventory.Singleton != null)
                Inventory.Singleton.OnSlotCountChanged -= OnSlotCountChanged;

            if (InputManager.Singleton != null)
                InputManager.Singleton.OnInventory -= ToggleInventory;
        }

        public void ToggleInventory()
        {
            isActiveInventory = !isActiveInventory;

            if (inventoryPanel != null)
                inventoryPanel.SetActive(isActiveInventory);
        }

        public void AddSlot()
        {
            Inventory.Singleton.AddSlot(1);
        }

        private void OnSlotCountChanged(int slotCount)
        {
            if (slots == null || slots.Length == 0)
                return;

            for (int i = 0; i < slots.Length; i++)
            {
                bool interactable = i < slotCount;
                slots[i].SetInteractable(interactable);
            }
        }
    }
}
