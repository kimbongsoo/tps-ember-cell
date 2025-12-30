using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public enum ItemType
    {
        Ammo,
        Heal,
        Buff,
        Key
    }

    [CreateAssetMenu(fileName = "InteractionDropItemData", menuName = "TEC/Interaction/Drop Item Data")]
    public class InteractionDropItemData : ScriptableObject, IInteractionData
    {
        public string ID => itemId;
        public Sprite ActionIcon => itemIcon;
        public string ActionMessage => itemName;

        public int ItemGrade => itemGrade;
        public ItemType ItemType => itemType;

        public int AmmoAmount => ammoAmount;
        public float HealAmount => healAmount;

        [SerializeField] private string itemId;
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private string itemName;
        [SerializeField] private int itemGrade;

        [Header("Item Type")]
        [SerializeField] private ItemType itemType = ItemType.Ammo;

        [Header("Ammo Setting")]
        [SerializeField] private int ammoAmount = 30;

        [Header("Heal Setting")]
        [SerializeField] private float healAmount = 25f;
    }
}
