using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TEC
{
    public class InventoryRenewalUI_ListEntity : MonoBehaviour, IPointerClickHandler
    {
        public UnityEngine.UI.Image itemIcon;
        public TMPro.TextMeshProUGUI itemNameText;
        public TMPro.TextMeshProUGUI itemCountText;
        public string ItemID { get; private set;} = string.Empty;
        public string DataID { get; private set;} = string.Empty;
        public void Init(string dataId, string itemId, Sprite icon, string itemName, int count)
        {
            DataID = dataId;
            ItemID = itemId;
            // TODO : Item Entity 초기화
            itemIcon.sprite = icon;
            itemNameText.text = itemName;
            itemCountText.text = $"x {count}";
        }

        //추가 우클릭은 메뉴 오픈 요청만
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null)
                return;

            if (eventData.button != PointerEventData.InputButton.Right)
                return;

            if (string.IsNullOrEmpty(ItemID))
                return;

            var menu = UIManager.Show<InventoryContextMenu>(UIList.InventoryContextMenu);
            if (menu != null)
                menu.Open(DataID, ItemID, transform as RectTransform);
        }
        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     Debug.Log("[Inventory] OnPointerClick CALLED"); // [ADDED] 무조건 찍히는지

        //     if (eventData == null)
        //     {
        //         Debug.Log("[Inventory] eventData is NULL"); // [ADDED]
        //         return;
        //     }

        //     Debug.Log($"[Inventory] button={eventData.button}, ItemID={ItemID}"); // [ADDED]

        //     if (eventData.button != PointerEventData.InputButton.Right)
        //         return;

        //     if (string.IsNullOrEmpty(ItemID))
        //     {
        //         Debug.Log("[Inventory] ItemID is empty"); // [ADDED]
        //         return;
        //     }

        //     // 우선 메뉴 호출은 주석 처리(이벤트만 확인)
        //     var menu = UIManager.Show<InventoryContextMenu>(UIList.InventoryContextMenu);
        //     Debug.Log($"[Inventory] ContextMenu = {(menu == null ? "NULL" : "OK")}"); // [ADDED]
        //     if (menu != null)
        //         menu.Open(ItemID, transform as RectTransform);

        //     Debug.Log("[Inventory] RIGHT CLICK OK"); // [ADDED]
        // }

    }
}
