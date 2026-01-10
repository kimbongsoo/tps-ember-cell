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
    }
    [CreateAssetMenu(fileName = "ItemDataSO", menuName = "PROJECT TEC/Game Data/Item Data")]
    public class ItemDataSO : ScriptableObject
    {
        public string ItemID;
        public string ItemName;
        public ItemCategory ItemCategory;
        public Sprite ItemIcon;
        
    }
}
