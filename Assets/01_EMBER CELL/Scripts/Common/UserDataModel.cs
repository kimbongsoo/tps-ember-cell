using UnityEngine;

namespace TEC
{
    public class UserDataModel : SingletonBase<UserDataModel>
    {
        [field: SerializeField] public PlayerItemDto PlayerItemData { get; private set; } = new();
        [field: SerializeField] public QuickSlotDto QuickSlotData { get; private set; } = new();


        public System.Action OnInventoryChanged;

        //아이템효과 적용
        public System.Action<ItemUseEffectType, float> OnItemEffectRequested;
        public System.Action OnQuickSlotChanged;
        public void Initialize()
        {
            // TODO : UserData Initialize / Load Logic
            InitializeQuickSlots();

            AddItem("40001", 10000);
        }

        private void InitializeQuickSlots()
        {
            if (QuickSlotData == null)
                QuickSlotData = new QuickSlotDto();

            if (QuickSlotData.slotItemIDs == null)
                QuickSlotData.slotItemIDs = new();

            while (QuickSlotData.slotItemIDs.Count < 2)
                QuickSlotData.slotItemIDs.Add(string.Empty);

            if (QuickSlotData.slotItemIDs.Count > 2)
                QuickSlotData.slotItemIDs.RemoveRange(2, QuickSlotData.slotItemIDs.Count - 2);
        }

        public int GetTotalItemCount(string itemID)
        {
            if (string.IsNullOrEmpty(itemID))
                return 0;

            int total = 0;
            var c = PlayerItemData.itemDataContainer;

            for (int i = 0; i < c.Count; i++)
            {
                if (c[i].itemID == itemID)
                    total += c[i].quantity;
            }

            return total;
        }

        // amount만큼
        public int ConsumeItem(string itemID, int amount)
        {
            if (string.IsNullOrEmpty(itemID) || amount <= 0)
                return 0;

            int remaining = amount;
            var c = PlayerItemData.itemDataContainer;

            for (int i = c.Count - 1; i >= 0 && remaining > 0; i--)
            {
                if (c[i].itemID != itemID)
                    continue;

                int take = Mathf.Min(c[i].quantity, remaining);
                c[i].quantity -= take;
                remaining -= take;

                if (c[i].quantity <= 0)
                    c.RemoveAt(i);
            }

            int consumed = amount - remaining;
            if (consumed > 0)
                OnInventoryChanged?.Invoke();
                CleanupQuickSlotsByInventory();

            return consumed;
        }

        // MaxStack초과시 분리
        public bool AddItem(string itemID, int amount)
        {
            if (string.IsNullOrEmpty(itemID) || amount <= 0)
                return false;

            var itemDataSO = GameDataModel.Singleton.ItemData.GetItemDataSO(itemID);

            if (itemDataSO == null)
                return false;

            int maxStack = itemDataSO.MaxStack;

            for (int i = 0; i < PlayerItemData.itemDataContainer.Count && amount > 0; i++)
            {
                var slot = PlayerItemData.itemDataContainer[i];
                if (slot.itemID != itemID)
                    continue;

                if (slot.quantity >= maxStack)
                    continue;

                int space = maxStack - slot.quantity;
                int add = Mathf.Min(space, amount);

                slot.quantity += add;
                amount -= add;
            }

            while (amount > 0)
            {
                int add = Mathf.Min(maxStack, amount);

                PlayerItemData.itemDataContainer.Add(new PlayerItemDto.PlayerItemData()
                {
                    dataID = System.Guid.NewGuid().ToString(),
                    itemID = itemID,
                    quantity = add
                });

                amount -= add;
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool TryUseItem(string itemID)
        {
            if (string.IsNullOrEmpty(itemID))
                return false;

            var itemDataSO = GameDataModel.Singleton.ItemData.GetItemDataSO(itemID);

            if (itemDataSO == null)
                return false;

            if (itemDataSO.UseEffectType == ItemUseEffectType.None)
                return false;

            if (itemDataSO.UseEffectValue <= 0f)
                return false;

            int consumed = ConsumeItem(itemID, 1);
            if (consumed <= 0)
                return false;

            OnItemEffectRequested?.Invoke(itemDataSO.UseEffectType, itemDataSO.UseEffectValue);
            return true;
        }

        public bool TryDropItem(string itemID, int amount)
        {
            if (string.IsNullOrEmpty(itemID) || amount <= 0)
                return false;

            int consumed = ConsumeItem(itemID, amount);
            return consumed > 0;
        }

        public bool TryDropAll(string itemID)
        {
            if (string.IsNullOrEmpty(itemID))
                return false;

            int total = GetTotalItemCount(itemID);
            if (total <= 0)
                return false;

            int consumed = ConsumeItem(itemID, total);
            return consumed > 0;
        }

        // dataID로 슬롯 삭제
        public bool TryDropByDataID(string dataID)
        {
            if (string.IsNullOrEmpty(dataID))
                return false;

            var c = PlayerItemData.itemDataContainer;

            for (int i = c.Count - 1; i >= 0; i--)
            {
                if (c[i].dataID != dataID)
                    continue;

                c.RemoveAt(i);
                OnInventoryChanged?.Invoke();
                return true;
            }

            return false;
        }

        public string GetQuickSlotItemID(int slotIndex)
        {
            InitializeQuickSlots();

            if (slotIndex < 0 || slotIndex >= 2)
                return string.Empty;

            return QuickSlotData.slotItemIDs[slotIndex];
        }

        // UseEffectType에 따라 등록
        public bool RegisterQuickSlotByEffect(string itemID)
        {
            if (string.IsNullOrEmpty(itemID))
                return false;

            if (GetTotalItemCount(itemID) <= 0)
                return false;

            var itemDataSO = GameDataModel.Singleton.ItemData.GetItemDataSO(itemID);
            if (itemDataSO == null)
                return false;

            InitializeQuickSlots();

            if (itemDataSO.UseEffectType == ItemUseEffectType.HealHP)
            {
                QuickSlotData.slotItemIDs[0] = itemID;
                OnQuickSlotChanged?.Invoke();
                return true;
            }

            if (itemDataSO.UseEffectType == ItemUseEffectType.RecoverSP)
            {
                QuickSlotData.slotItemIDs[1] = itemID;
                OnQuickSlotChanged?.Invoke();
                return true;
            }

            return false;
        }

        public bool TryUseQuickSlot(int slotIndex)
        {
            string itemID = GetQuickSlotItemID(slotIndex);
            if (string.IsNullOrEmpty(itemID))
                return false;

            return TryUseItem(itemID);
        }

        private void CleanupQuickSlotsByInventory()
        {
            InitializeQuickSlots();

            bool changed = false;

            for (int i = 0; i < 2; i++)
            {
                string itemID = QuickSlotData.slotItemIDs[i];
                if (string.IsNullOrEmpty(itemID))
                    continue;

                if (GetTotalItemCount(itemID) <= 0)
                {
                    QuickSlotData.slotItemIDs[i] = string.Empty;
                    changed = true;
                }
            }

            if (changed)
                OnQuickSlotChanged?.Invoke();
        }

    }
}

