using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class GameDataDto { }

    [System.Serializable]
    public class ItemDataDto : GameDataDto
    {
        [System.Serializable]
        public class ItemData
        {
            public string id;
            public ScriptableObject itemSO;
        }

        public List<ItemData> itemDataContainer = new();
        
        public ItemDataSO GetItemDataSO(string id)
        {
            var targetItemData = itemDataContainer.Find(x => x.id.Equals(id));
            if (targetItemData != null)
            {
                var itemDataSO = targetItemData.itemSO as ItemDataSO;
                return itemDataSO;
            }

            return null;
        }
    }
}
