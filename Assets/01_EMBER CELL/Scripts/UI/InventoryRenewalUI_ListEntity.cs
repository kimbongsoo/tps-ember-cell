using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class InventoryRenewalUI_ListEntity : MonoBehaviour
    {
        public UnityEngine.UI.Image itemIcon;
        public TMPro.TextMeshProUGUI itemNameText;
        public TMPro.TextMeshProUGUI itemCountText;

        public void Init(Sprite icon, string itemName, int count)
        {
            // TODO : Item Entity 초기화
            itemIcon.sprite = icon;
            itemNameText.text = itemName;
            itemCountText.text = $"x {count}";
        }
    }
}
