using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class Inventory : SingletonBase<Inventory>
    {
        public delegate void OnSlotCountChange(int val);
        public OnSlotCountChange onSlotCountChange;

        public delegate void OnchangeItem();
        public OnchangeItem onChangeItem;

        public List<InteractionDropItemData> items = new List<InteractionDropItemData>();

        private int slotCnt;
        public int SlotCnt
        {
            get => slotCnt;
            set
            {
                slotCnt = value;
                onSlotCountChange?.Invoke(slotCnt);
            }
        }

        private void Start()
        {
            SlotCnt = 16;
        }

        // public bool AddItem(InteractionDropItemData _item)
        // {
        //     if (_item == null)
        //         return false;

        //     if (items.Count < SlotCnt)
        //     {
        //         items.Add(_item);
        //         onChangeItem?.Invoke();
        //         return true;
        //     }
        //     return false;
        // }
        public bool AddItem(InteractionDropItemData _item)
        {
            Debug.Log($"[Inventory] AddItem called. item={(_item == null ? "null" : _item.name)} count={items.Count}/{SlotCnt}", this);

            if (_item == null)
                return false;

            if (items.Count < SlotCnt)
            {
                items.Add(_item);
                Debug.Log($"[Inventory] AddItem SUCCESS. newCount={items.Count}", this);
                Debug.Log("[Inventory] Invoke onChangeItem", this);

                onChangeItem?.Invoke();
                return true;
            }

            Debug.Log("[Inventory] AddItem FAIL (Full)", this);
            return false;
        }


        //TODO : F로 아이템 먹었을 떄 아이템의 정보를 인자로 넘겨 주는 코드
    }
}

