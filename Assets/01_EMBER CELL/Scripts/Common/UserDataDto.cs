using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class UserDataDto{ }

    [System.Serializable]
    public class PlayerItemDto : UserDataDto
    {
        [System.Serializable]
        public class PlayerItemData
        {
            public string dataID;
            public string itemID;
            public int quantity;
        }

        public List<PlayerItemData> itemDataContainer = new();
    }
}
