using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class ItemDatabase : SingletonBase<ItemDatabase>
    {
        private readonly Dictionary<string, InteractionDropItemData> guidToData = new Dictionary<string, InteractionDropItemData>();

        private bool isInitialized = false;

        private const string ITEM_DATA_PATH = "Items";

        public void Initialize()
        {
            if (isInitialized)
                return;

            guidToData.Clear();

            var allItems = Resources.LoadAll<InteractionDropItemData>(ITEM_DATA_PATH);
            foreach (var item in allItems)
            {
                if (item == null)
                    continue;

                if (string.IsNullOrEmpty(item.Guid))
                {
                    Debug.LogWarning($"[ItemDatabase] Item guid is empty : {item.name}");
                    continue;
                }

                if (guidToData.ContainsKey(item.Guid))
                {
                    Debug.LogWarning($"[ItemDatabase] Duplicated guid : {item.Guid} / {item.name}");
                    continue;
                }

                guidToData.Add(item.Guid, item);
            }

            isInitialized = true;
        }

        public bool TryGetItem(string guid, out InteractionDropItemData itemData)
        {
            if (!isInitialized)
                Initialize();

            return guidToData.TryGetValue(guid, out itemData);
        }
    }
}
