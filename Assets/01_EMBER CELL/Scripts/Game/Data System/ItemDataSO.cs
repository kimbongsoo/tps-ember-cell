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

    //추가
    public enum ItemUseEffectType
    {
        None = 0,
        HealHP = 1,
        RecoverSP = 2,
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
        [Header("Use Effect")]
        public ItemUseEffectType UseEffectType = ItemUseEffectType.None;
        public float UseEffectValue = 0f;

        [Header("World Visual")]
        public GameObject WorldPrefab;
        
        public string ID => ItemID;
        public Sprite ActionIcon => ItemIcon;
        public string ActionMessage => ItemName;

        

#if UNITY_EDITOR
private void OnValidate()
{
    MaxStack = ItemCategory switch
    {
        ItemCategory.Ammo => 100,
        ItemCategory.Consumable => 10,
        _ => 1
    };

    DropQuantity = ItemCategory switch
    {
        ItemCategory.Ammo => 30,
        ItemCategory.Consumable => 5,
        _ => 1
    };
}
#endif
        
    }
}
