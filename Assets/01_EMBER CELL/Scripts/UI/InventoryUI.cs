using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    public class InventoryUI : UIBase
    {
        Inventory inven;
        public GameObject inventoryPanel;
        bool activeInventory = false;

        public Slot[] slots;
        public Transform slotHolder;


        private void Start()
        {
            inven = Inventory.Singleton;
            slots = slotHolder.GetComponentsInChildren<Slot>(true);

            inven.onSlotCountChange += SlotChange;
            inven.onChangeItem += ReDrawSlotUI;
            inventoryPanel.SetActive(activeInventory);

            SlotChange(inven.SlotCnt);
            ReDrawSlotUI();
        }

        private void OnDestroy()
        {
            if (inven == null)
                return;

            inven.onSlotCountChange -= SlotChange;
            inven.onChangeItem -= ReDrawSlotUI;
        }

        private void SlotChange(int val)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                    continue;

                var btn = slots[i].GetComponent<Button>();
                if (btn == null)
                    continue;

                btn.interactable = i < inven.SlotCnt;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                activeInventory = !activeInventory;
                inventoryPanel.SetActive(activeInventory);

                // 열 때 갱신
                ReDrawSlotUI();
            }
        }

        public void AddSlot()
        {
            inven.SlotCnt++;
        }

        void ReDrawSlotUI()
        {
            Debug.Log($"[InventoryUI] ReDrawSlotUI ENTER items.Count={(inven == null ? -1 : inven.items.Count)}", this);
            if (slots == null)
                return;

            // 1) 전체 초기화(Null 안전)
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null)
                    continue;

                slots[i].RemoveSlot();
            }

            // 2) 아이템 표시(슬롯 길이/슬롯수 초과 방지)
            if (inven == null || inven.items == null)
                return;

            int count = inven.items.Count;
            if (count > slots.Length) count = slots.Length;
            if (count > inven.SlotCnt) count = inven.SlotCnt;

            for (int i = 0; i < count; i++)
            {
                if (slots[i] == null)
                    continue;

                var it = inven.items[i];
                slots[i].item = it;
                slots[i].UpdateSlotUI();
            }
        }
    }
}
