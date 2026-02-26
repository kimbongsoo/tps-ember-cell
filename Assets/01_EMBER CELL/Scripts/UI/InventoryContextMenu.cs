using UnityEngine;

namespace TEC
{
    public class InventoryContextMenu : UIBase
    {
        public static InventoryContextMenu Instance => UIManager.Singleton.GetUI<InventoryContextMenu>(UIList.InventoryContextMenu);
        [SerializeField] private RectTransform root;
        private string currentItemID = string.Empty;
        private string currentDataID = string.Empty;

        public void Open(string dataId, string itemID, RectTransform anchor)
        {
            currentDataID = dataId;
            currentItemID = itemID;

            // TODO: 위치 배치가 필요하면 다음 단계에서 추가
            // 우선은 표시만
            Show();
            transform.SetAsLastSibling();

            if ( root == null)
                return;
            // MenuRoot를 마우스 위치로 이동
            root.position = Input.mousePosition;
        }

        public void OnClickUse()
        {
            if (string.IsNullOrEmpty(currentItemID))
                return;

            UserDataModel.Singleton.TryUseItem(currentItemID);
            Hide();
        }

        public void OnClickDrop()
        {
            if (string.IsNullOrEmpty(currentDataID))
                return;

            UserDataModel.Singleton.TryDropByDataID(currentDataID);

            Hide();
        }

        public void OnClickRegisterQuickSlot()
        {
            if (string.IsNullOrEmpty(currentItemID))
                return;

            UserDataModel.Singleton.RegisterQuickSlotByEffect(currentItemID);
            // TODO: 다음 단계에서 UserDataModel.RegisterQuickSlot(slotIndex, itemID) 연결
            Hide();
        }
    }
}
