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

    //퀵슬롯 추가
    [System.Serializable]
    public class QuickSlotDto : UserDataDto
    {
        // 0 = HP 회복 아이템, 1 = SP 회복 아이템
        public List<string> slotItemIDs = new() { string.Empty, string.Empty };
    }
}
