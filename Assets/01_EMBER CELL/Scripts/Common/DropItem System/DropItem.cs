// using System.Collections.Generic;
// using UnityEngine;

// namespace TEC
// {
//     public class DropItem : MonoBehaviour, IInteractionProvider
//     {
//         public List<IInteractionData> Interactions => interactionDatas;
//         public void Interact(IInteractionData data)
//         {
            
//         }

//         private readonly List<IInteractionData> interactionDatas = new();

//         [SerializeField] private ItemDataSO itemDataSO;
//         [SerializeField] private int quantity = 1;

//         public ItemDataSO ItemDataSO => itemDataSO;
//         public string ItemID => itemDataSO != null ? itemDataSO.ItemID : string.Empty;
//         public int Quantity => quantity;

//         private void Awake()
//         {
//             interactionDatas.Add(ItemDataSO);
//         }
//     }
// }
