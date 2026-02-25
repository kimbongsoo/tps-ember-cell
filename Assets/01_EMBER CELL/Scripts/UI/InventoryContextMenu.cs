using UnityEngine;

namespace TEC
{
    public class InventoryContextMenu : UIBase
    {
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

        // 버튼 OnClick에 연결
        public void OnClickUse()
        {
            if (string.IsNullOrEmpty(currentItemID))
                return;

            UserDataModel.Singleton.TryUseItem(currentItemID);
            Hide();
        }

        // 버튼 OnClick에 연결 (1개 버리기)
        public void OnClickDrop()
        {
            if (string.IsNullOrEmpty(currentDataID))
                return;

            UserDataModel.Singleton.TryDropByDataID(currentDataID);

            Hide();
        }

        // 버튼 OnClick에 연결 (퀵슬롯 등록: 데이터만, UI 없음)
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
