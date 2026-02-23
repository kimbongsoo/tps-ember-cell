using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class GameDataModel : SingletonBase<GameDataModel>
    {
        [field:SerializeField] public ItemDataDto ItemData {get; private set;} = new();

        public void Initialize()
        {
            //TODO : GameData Initialize / Load Logic
            ItemDataSO[] loadedData = Resources.LoadAll<ItemDataSO>("Game Data/Item Data/");
            for(int i = 0; i < loadedData.Length; i++ )
            {
                ItemData.itemDataContainer.Add(new ItemDataDto.ItemData()
                {
                   id = loadedData[i].ItemID,
                   itemSO = loadedData[i], 
                });
            }
        }

        //추가
        public ItemDataSO GetItemData(string itemID)
        {
            return ItemData.GetItemDataSO(itemID);
        }


    }
}
