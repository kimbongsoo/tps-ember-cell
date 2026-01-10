using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class UserDataModel : SingletonBase<UserDataModel>
    {
        [field: SerializeField] public PlayerItemDto PlayerItemData { get; private set;} = new();

        public void Initialize()
        {
            // Dummy Sample Data Create
            int totalItemCnt = GameDataModel.Singleton.ItemData.itemDataContainer.Count;
            for(int i = 0; i < totalItemCnt; i++)
            {
                PlayerItemData.itemDataContainer.Add(new PlayerItemDto.PlayerItemData()
                {
                    dataID = System.Guid.NewGuid().ToString(),
                    itemID = GameDataModel.Singleton.ItemData.itemDataContainer[i].id,
                    quantity = Random.Range(1, 10),
                });
            }
            

            //TODO : UserData Initialize / Load Logic

        }
    }
}
