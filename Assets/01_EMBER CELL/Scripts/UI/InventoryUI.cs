using UnityEngine;
using UnityEngine.UI;

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
            OnSlotCountChanged(Inventory.Singleton.SlotCount);
        }

        private void OnDisable()
        {
            if (Inventory.Singleton != null)
                Inventory.Singleton.OnSlotCountChanged -= OnSlotCountChanged;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                ToggleInventory();
            }
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

                // Slot에 SetInteractable이 없다면 아래 2줄을 주석 해제하고 사용하세요.
                // var button = slots[i].GetComponent<Button>();
                // if (button != null) button.interactable = interactable;

                slots[i].SetInteractable(interactable);
            }
        }
    }
}
