using UnityEngine;

namespace TEC
{
    public class UserDataModel : SingletonBase<UserDataModel>
    {
        [field: SerializeField] public PlayerItemDto PlayerItemData { get; private set; } = new();

        public System.Action OnInventoryChanged;

        public void Initialize()
        {
            // TODO : UserData Initialize / Load Logic

            // 더미 데이터는 비활성화
            /*
            int totalItemCnt = GameDataModel.Singleton.ItemData.itemDataContainer.Count;
            for (int i = 0; i < totalItemCnt; i++)
            {
                PlayerItemData.itemDataContainer.Add(new PlayerItemDto.PlayerItemData()
                {
                    dataID = System.Guid.NewGuid().ToString(),
                    itemID = GameDataModel.Singleton.ItemData.itemDataContainer[i].id,
                    quantity = Random.Range(1, 10),
                });
            }
            */
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



    }
}

