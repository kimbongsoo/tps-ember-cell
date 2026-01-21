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

        public bool AddItem(string itemID, int amount)
        {
            if (string.IsNullOrEmpty(itemID) || amount <= 0)
                return false;

            var itemDataSO = GameDataModel.Singleton.ItemData.GetItemDataSO(itemID);
            if (itemDataSO == null)
                return false;

            int maxStack = itemDataSO.MaxStack;

            // 기존 스택 찾기
            var existing = PlayerItemData.itemDataContainer
                .Find(x => x.itemID == itemID && x.quantity < maxStack);

            if (existing != null)
            {
                int space = maxStack - existing.quantity;
                int add = Mathf.Min(space, amount);

                existing.quantity += add;
                amount -= add;
            }

            // 남은 수량이 있다면 새 스택 생성
            while (amount > 0)
            {
                int add = Mathf.Min(maxStack, amount);

                PlayerItemData.itemDataContainer.Add(new PlayerItemDto.PlayerItemData
                {
                    dataID = System.Guid.NewGuid().ToString(),
                    itemID = itemID,
                    quantity = add
                });

                amount -= add;
            }

            return true;
        }

    }
}

