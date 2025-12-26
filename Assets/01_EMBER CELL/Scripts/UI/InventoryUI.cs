using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TEC
{
    public class InventoryUI : UIBase
    {
        public static InventoryUI Instance => UIManager.Singleton.GetUI<InventoryUI>(UIList.InventoryUI);
        
        Inventory inven;

        public GameObject inventoryPanel;
        bool activeInventory = false;

        public Slot[] slots;
        public Transform slotHolder;

        private void Start()
        {
            inven = Inventory.instance;
            slots = slotHolder.GetComponentsInChildren<Slot>();
            inven.onSlotCountChange += SlotChange;
            inventoryPanel.SetActive(activeInventory);
        }

        private void SlotChange(int val)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if(i < inven.SlotCnt)
                    slots[i].GetComponent<Button>().interactable = true;
                else
                    slots[i].GetComponent<Button>().interactable = false;
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.I))
            {
                activeInventory = !activeInventory;
                inventoryPanel.SetActive(activeInventory);
            }
        }

        public void AddSlot()
        {
            inven.SlotCnt++;
        }
    }
}
