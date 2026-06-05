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
                menu.Open(DataID, ItemID, (RectTransform)transform);
        }

    }
}
