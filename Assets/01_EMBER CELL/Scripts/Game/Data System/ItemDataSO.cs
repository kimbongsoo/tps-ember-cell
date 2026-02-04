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
    public class ItemDataSO : ScriptableObject, IInteractionData
    {
        public string ItemID;
        public string ItemName;
        public ItemCategory ItemCategory;
        public Sprite ItemIcon;
        public int MaxStack;
        public int DropQuantity;

        //추가
        public string ID => ItemID;
        public Sprite ActionIcon => ItemIcon;
        public string ActionMessage => ItemName;

        

#if UNITY_EDITOR
private void OnValidate()
{
    MaxStack = ItemCategory switch
    {
        ItemCategory.Ammo => 100,
        _ => 1
    };

    DropQuantity = ItemCategory switch
    {
        ItemCategory.Ammo => 30,
        _ => 1
    };
}
#endif
        
    }
}
