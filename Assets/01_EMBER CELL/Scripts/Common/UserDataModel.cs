// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace TEC
// {
//     public class UserDataModel : SingletonBase<UserDataModel>
//     {
//         [field: SerializeField] public PlayerItemDto PlayerItemData { get; private set;} = new();

//         public void Initialize()
//         {
//             // Dummy Sample Data Create
//             int totalItemCnt = GameDataModel.Singleton.ItemData.itemDataContainer.Count;
//             for(int i = 0; i < totalItemCnt; i++)
//             {
//                 PlayerItemData.itemDataContainer.Add(new PlayerItemDto.PlayerItemData()
//                 {
//                     dataID = System.Guid.NewGuid().ToString(),
//                     itemID = GameDataModel.Singleton.ItemData.itemDataContainer[i].id,
//                     quantity = Random.Range(1, 10),
//                 });
//             }
//             //TODO : UserData Initialize / Load Logic
//         }
//     }
// }
using UnityEngine;

namespace TEC
{
    public class UserDataModel : SingletonBase<UserDataModel>
    {
        [field: SerializeField] public PlayerItemDto PlayerItemData { get; private set; } = new();

        public void Initialize()
        {
            // TODO : UserData Initialize / Load Logic

            // 더미 데이터는 일단 비활성화 (줍기 테스트를 위해)
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

        public bool AddItem(string itemId, int amount)
        {
            if (string.IsNullOrEmpty(itemId))
                return false;

            if (amount <= 0)
                return false;

            var container = PlayerItemData.itemDataContainer;
            var existed = container.Find(x => x.itemID == itemId);

            if (existed != null)
            {
                existed.quantity += amount;
            }
            else
            {
                container.Add(new PlayerItemDto.PlayerItemData()
                {
                    dataID = System.Guid.NewGuid().ToString(),
                    itemID = itemId,
                    quantity = amount,
                });
            }

            return true;
        }
    }
}

