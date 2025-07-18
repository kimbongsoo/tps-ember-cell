// using System.Collections.Generic;
// using Gpm.Ui;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// namespace KBS
// {
//     public class InteractionUI_ListItemData : InfiniteScrollData
//     {
//         public string id;
//         public Sprite icon;
//         public string message;
//         public int stackCount;
//         public bool isSelected;

//     }
//     public class InteractionUI_ListItem : InfiniteScrollItem
//     {
//         [SerializeField] private Image iconImage;
//         [SerializeField] private TextMeshProUGUI messageText;
//         [SerializeField] private GameObject selection;
//         public override void UpdateData(InfiniteScrollData scrollData)
//         {
//             var listData = scrollData as InteractionUI_ListItemData;
//             iconImage.sprite = listData.icon;
//             messageText.text = listData.message;

//             selection.SetActive(listData.isSelected);
//         }

//     }
// }
