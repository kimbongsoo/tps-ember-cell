using UnityEngine;

namespace TEC
{
    public class DropItem : MonoBehaviour
    {
        [SerializeField] private ItemDataSO itemDataSO;
        [SerializeField] private int quantity = 1;
        [SerializeField] private MeshRenderer visualRenderer;

        public ItemDataSO ItemDataSO => itemDataSO;
        public string ItemID => itemDataSO != null ? itemDataSO.ItemID : string.Empty;
        public int Quantity => quantity;
    }
}
