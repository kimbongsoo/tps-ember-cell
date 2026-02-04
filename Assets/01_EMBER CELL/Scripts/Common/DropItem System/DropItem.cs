using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class DropItem : MonoBehaviour, IInteractionProvider
    {
        //추가
        public List<IInteractionData> Interactions => interactionDatas;
        public void Interact(IInteractionData data)
        {
            
        }

        private readonly List<IInteractionData> interactionDatas = new();

        // 여기까지
        [SerializeField] private ItemDataSO itemDataSO;
        [SerializeField] private int quantity = 1;

        public ItemDataSO ItemDataSO => itemDataSO;
        public string ItemID => itemDataSO != null ? itemDataSO.ItemID : string.Empty;
        public int Quantity => quantity;

        //추가
        private void Awake()
        {
            interactionDatas.Add(ItemDataSO);
        }
    }
}
