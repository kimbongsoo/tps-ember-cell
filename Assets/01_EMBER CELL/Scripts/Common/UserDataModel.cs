using UnityEngine;

namespace TEC
{
    public class UserDataModel : SingletonBase<UserDataModel>
    {
        [field: SerializeField] public PlayerItemDto PlayerItemData { get; private set; } = new();

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

        // public bool AddItem(string itemId, int amount)
        // {
        //     if (string.IsNullOrEmpty(itemId))
        //         return false;

        //     if (amount <= 0)
        //         return false;

        //     var container = PlayerItemData.itemDataContainer;
        //     var existed = container.Find(x => x.itemID == itemId);

        //     if (existed != null)
        //     {
        //         existed.quantity += amount;
        //     }
        //     else
        //     {
        //         container.Add(new PlayerItemDto.PlayerItemData()
        //         {
        //             dataID = System.Guid.NewGuid().ToString(),
        //             itemID = itemId,
        //             quantity = amount,
        //         });
        //     }

        //     return true;
        // }

        // public bool AddItem(string itemID, int amount)
        // {
        //     if (string.IsNullOrEmpty(itemID) || amount <= 0)
        //         return false;

        //     var itemDataSO = GameDataModel.Singleton.ItemData.GetItemDataSO(itemID);
        //     if (itemDataSO == null)
        //         return false;

        //     int maxStack = itemDataSO.MaxStack;

        //     // 기존 스택 찾기
        //     var existing = PlayerItemData.itemDataContainer
        //         .Find(x => x.itemID == itemID && x.quantity < maxStack);

        //     if (existing != null)
        //     {
        //         int space = maxStack - existing.quantity;
        //         int add = Mathf.Min(space, amount);

        //         existing.quantity += add;
        //         amount -= add;
        //     }

        //     // 남은 수량이 있다면 새 스택 생성
        //     while (amount > 0)
        //     {
        //         int add = Mathf.Min(maxStack, amount);

        //         PlayerItemData.itemDataContainer.Add(new PlayerItemDto.PlayerItemData
        //         {
        //             dataID = System.Guid.NewGuid().ToString(),
        //             itemID = itemID,
        //             quantity = add
        //         });

        //         amount -= add;
        //     }

        //     return true;
        // }
        public bool AddItem(string itemID, int amount)
        {
            if (string.IsNullOrEmpty(itemID) || amount <= 0)
                return false;

            var itemDataSO = GameDataModel.Singleton.ItemData.GetItemDataSO(itemID);
            if (itemDataSO == null)
                return false;

            int maxStack = itemDataSO.MaxStack;

            // 1️⃣ 기존 스택 중 "아직 덜 찬 스택"부터 채운다
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

            // 2️⃣ 아직 남은 수량이 있으면 새 스택을 만든다
            while (amount > 0)
            {
                int add = Mathf.Min(maxStack, amount);

                PlayerItemData.itemDataContainer.Add(
                    new PlayerItemDto.PlayerItemData
                    {
                        dataID = System.Guid.NewGuid().ToString(),
                        itemID = itemID,
                        quantity = add
                    }
                );

                amount -= add;
            }

            return true;
        }


    }
}

