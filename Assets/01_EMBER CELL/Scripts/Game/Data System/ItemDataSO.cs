using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public enum ItemCategory
    {
        None = 0,

        Equipment = 1,
        Consumable = 2,
        Material = 3,
        Ammo = 4,
    }
    [CreateAssetMenu(fileName = "ItemDataSO", menuName = "PROJECT TEC/Game Data/Item Data")]
    public class ItemDataSO : ScriptableObject
    {
        public string ItemID;
        public string ItemName;
        public ItemCategory ItemCategory;
        public Sprite ItemIcon;
        public int MaxStack;

#if UNITY_EDITOR
        private void OnValidate()
        {
            switch (ItemCategory)
            {
                case ItemCategory.Equipment:
                    MaxStack = 1;
                    break;

                case ItemCategory.Ammo:
                    MaxStack = 100;
                    break;
            }

            if (MaxStack < 1)
                MaxStack = 1;
        }
#endif
        
    }
}
