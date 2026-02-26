using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TEC
{
    public class InventoryRenewalUI : UIBase
    {
        public static InventoryRenewalUI Instance => UIManager.Singleton.GetUI<InventoryRenewalUI>(UIList.InventoryRenewalUI);
        public Transform listRoot; //하이어러키 상의, Content 오브젝트
        public InventoryRenewalUI_ListEntity itemListEntity; // 프리팹오브젝트 

        private void Awake()
        {
            itemListEntity.gameObject.SetActive(false);
        }
        public override void Show()
        {
            base.Show();
            InputManager.Singleton.SetCursorForcedByUI(true, true);
            Refresh();

        }

        public override void Hide()
        {
            base.Hide();
            InputManager.Singleton.SetCursorForcedByUI(false, false);

            if (InventoryContextMenu.Instance != null && InventoryContextMenu.Instance.gameObject.activeSelf)
                InventoryContextMenu.Instance.Hide();
        }

        //추가
        public void Refresh()
        {
            ClearList();

            // TODO : UserData에 있는 Player Item 정보를 토대로, UI를 갱신한다.
            for(int i=0; i < UserDataModel.Singleton.PlayerItemData.itemDataContainer.Count; i++)
            {
                string dataId = UserDataModel.Singleton.PlayerItemData.itemDataContainer[i].dataID;
                string itemId = UserDataModel.Singleton.PlayerItemData.itemDataContainer[i].itemID;
                int count = UserDataModel.Singleton.PlayerItemData.itemDataContainer[i].quantity;

                AddItem(dataId, itemId, count);
            }
            
        }

        //0119 ClearList
        private void ClearList()
        {
            // listRoot 하위의 실제 생성된 엔티티만 제거
            for (int i = listRoot.childCount - 1; i >= 0; i--)
            {
                var child = listRoot.GetChild(i);
                if (child == itemListEntity.transform)
                    continue;

                Destroy(child.gameObject);
            }
        }

        public void AddItem(string dataId, string itemId, int count)
        {
            //TODO : UI상에, itemListEntity를 복제해서 추가..

            var itemDataSO = GameDataModel.Singleton.ItemData.GetItemDataSO(itemId);
            if (itemDataSO == null)
                return;

            InventoryRenewalUI_ListEntity newItemEntity = Instantiate(itemListEntity, listRoot);
            newItemEntity.gameObject.SetActive(true);

            newItemEntity.Init(dataId, itemId, itemDataSO.ItemIcon, itemDataSO.ItemName, count);
        }

        public void RemoveItem()
        {
            //TODO : UI상에, itemListEntity를 제거
        }

        public void UpdateItem()
        {
            //TODO : UI상에, itemListEntity의 데이터를 갱신
        }

        public void CloseUI()
        {
            UIManager.Hide<InventoryRenewalUI>(UIList.InventoryRenewalUI);
        }
    }
}
