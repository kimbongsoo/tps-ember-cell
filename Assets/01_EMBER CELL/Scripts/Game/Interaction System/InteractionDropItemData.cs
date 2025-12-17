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

        // =========================
        // Inventory / Save Key
        // =========================
        public string Guid => guid;

        // =========================
        // Inventory Settings
        // =========================
        public ItemType Type => itemType;
        public bool IsStackable => isStackable;
        public int MaxStack => maxStack;

        // 퀵슬롯에 올릴 수 있는 아이템인지 (추후 확장 대비)
        public bool IsQuickSlotAllowed => isQuickSlotAllowed;

        // =========================
        // Item Effects (v1)
        // =========================
        public int AmmoAmount => ammoAmount;
        public float HealAmount => healAmount;

        [Header("Common")]
        [SerializeField] private string itemId;
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private string itemName;
        [SerializeField] private int itemGrade;

        [Header("Inventory")]
        [SerializeField] private string guid; // 저장/로드 기준 키
        [SerializeField] private ItemType itemType = ItemType.Ammo;
        [SerializeField] private bool isStackable = true;
        [SerializeField] private int maxStack = 99;
        [SerializeField] private bool isQuickSlotAllowed = true;

        [Header("Effect - Ammo")]
        [SerializeField] private int ammoAmount = 30;

        [Header("Effect - Heal")]
        [SerializeField] private float healAmount = 25f;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(guid))
            {
                guid = System.Guid.NewGuid().ToString("N");
                UnityEditor.EditorUtility.SetDirty(this);
            }

            if (isStackable == false)
            {
                maxStack = 1;
            }
            else
            {
                if (maxStack < 1) maxStack = 1;
            }
        }
#endif
    }
}
