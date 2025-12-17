using System;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance { get; private set; }

        public event Action OnInventoryChanged;

        [Header("Capacity")]
        [SerializeField] private int initialCapacity = 24;
        [SerializeField] private int maxCapacity = 64;
        [SerializeField] private int currentCapacity = 24;

        [Header("Slots")]
        [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();

        [Header("QuickSlot (v1 구조만)")]
        [SerializeField] private int quickSlotCount = 4;
        [SerializeField] private string[] quickSlotGuids;

        public IReadOnlyList<InventorySlot> Slots => slots;
        public int CurrentCapacity => currentCapacity;
        public int MaxCapacity => maxCapacity;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (quickSlotGuids == null || quickSlotGuids.Length != quickSlotCount)
                quickSlotGuids = new string[quickSlotCount];

            currentCapacity = Mathf.Clamp(currentCapacity, initialCapacity, maxCapacity);
        }

        // 용량 확장 (예: 가방 확장 아이템/업그레이드로 호출)
        public bool TryExpandCapacity(int addCount)
        {
            if (addCount <= 0)
                return false;

            int prev = currentCapacity;
            currentCapacity = Mathf.Clamp(currentCapacity + addCount, initialCapacity, maxCapacity);

            bool changed = prev != currentCapacity;
            if (changed)
                OnInventoryChanged?.Invoke();

            return changed;
        }

        // 인벤토리에 추가
        public bool TryAddItem(InteractionDropItemData itemData, int amount)
        {
            if (itemData == null || amount <= 0)
                return false;

            if (string.IsNullOrEmpty(itemData.Guid))
                return false;

            // Key 아이템은 1개만 유지
            if (itemData.Type == ItemType.Key)
            {
                if (Contains(itemData.Guid))
                    return false;

                return TryAddNewSlot(itemData.Guid, 1);
            }

            // 이미 있는 슬롯이면 스택 증가
            var slot = FindSlot(itemData.Guid);
            if (slot != null)
            {
                int maxStack = Mathf.Max(1, itemData.MaxStack);
                slot.amount = Mathf.Clamp(slot.amount + amount, 1, maxStack);

                OnInventoryChanged?.Invoke();
                return true;
            }

            // 없으면 새 슬롯 추가
            return TryAddNewSlot(itemData.Guid, amount);
        }

        private bool TryAddNewSlot(string guid, int amount)
        {
            if (slots.Count >= currentCapacity)
            {
                Debug.Log("인벤토리가 가득 찼습니다.");
                return false;
            }

            slots.Add(new InventorySlot(guid, amount));
            OnInventoryChanged?.Invoke();
            return true;
        }

        // 제거
        public bool TryRemoveItem(string guid, int amount)
        {
            if (string.IsNullOrEmpty(guid) || amount <= 0)
                return false;

            var slot = FindSlot(guid);
            if (slot == null)
                return false;

            slot.amount -= amount;
            if (slot.amount <= 0)
            {
                slots.Remove(slot);
                ClearQuickSlotIfMatched(guid);
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool Contains(string guid)
        {
            return FindSlot(guid) != null;
        }

        public int GetCount(string guid)
        {
            var slot = FindSlot(guid);
            return slot != null ? slot.amount : 0;
        }

        private InventorySlot FindSlot(string guid)
        {
            return slots.Find(s => s.guid == guid);
        }

        // =========================
        // QuickSlot (구조만)
        // =========================
        public bool TryAssignQuickSlot(int index, string guid)
        {
            if (quickSlotGuids == null)
                return false;

            if (index < 0 || index >= quickSlotGuids.Length)
                return false;

            if (!Contains(guid))
                return false;

            quickSlotGuids[index] = guid;
            OnInventoryChanged?.Invoke();
            return true;
        }

        public string GetQuickSlotGuid(int index)
        {
            if (quickSlotGuids == null)
                return string.Empty;

            if (index < 0 || index >= quickSlotGuids.Length)
                return string.Empty;

            return quickSlotGuids[index];
        }

        private void ClearQuickSlotIfMatched(string guid)
        {
            if (quickSlotGuids == null)
                return;

            for (int i = 0; i < quickSlotGuids.Length; i++)
            {
                if (quickSlotGuids[i] == guid)
                    quickSlotGuids[i] = string.Empty;
            }
        }

        public bool TryUseItem(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return false;

            if (!Contains(guid))
                return false;

            if (ItemDatabase.Singleton.TryGetItem(guid, out var itemData) == false)
                return false;

            // Key 아이템은 보통 "사용" 개념이 아니라 "소지 여부"로 체크하는 용도
            if (itemData.Type == ItemType.Key)
                return false;

            bool isUsed = false;

            switch (itemData.Type)
            {
                case ItemType.Ammo:
                    {
                        isUsed = TryApplyAmmo(itemData);
                    }
                    break;

                case ItemType.Heal:
                    {
                        isUsed = TryApplyHeal(itemData);
                    }
                    break;

                case ItemType.Buff:
                    {
                        // v1: 버프는 다음 단계에서 구현
                        // 현재는 사용만 막아두거나, 테스트용으로 true 처리 가능
                        isUsed = false;
                    }
                    break;
            }

            if (isUsed)
            {
                // 사용 성공 시 1개 소모
                TryRemoveItem(guid, 1);
                return true;
            }

            return false;
        }

        private bool TryApplyAmmo(InteractionDropItemData itemData)
        {
            if (itemData == null || itemData.AmmoAmount <= 0)
                return false;

            var player = CharacterPlayerController.Instance;
            if (player == null)
                return false;

            var character = player.GetComponent<CharacterBase>();
            if (character == null || character.PrimaryWeapon == null)
                return false;

            // WeaponBase가 예비탄 구조라면 AddAmmo가 reserveAmmo를 올리게 되어 있어야 함
            character.PrimaryWeapon.AddAmmo(itemData.AmmoAmount, out int current, out int reserve);

            MainHUD.Instance.SetAmmoText(current, reserve);

            return true;
        }

        private bool TryApplyHeal(InteractionDropItemData itemData)
        {
            if (itemData == null || itemData.HealAmount <= 0f)
                return false;

            var player = CharacterPlayerController.Instance;
            if (player == null)
                return false;

            var character = player.GetComponent<CharacterBase>();
            if (character == null)
                return false;

            // HP회복
            character.HealHP(itemData.HealAmount);

            return true;
        }

    }
}
