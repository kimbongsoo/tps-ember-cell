using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TEC
{
    public class InventoryRenewalUI : UIBase
    {
        public Transform listRoot; //하이어러키 상의, Content 오브젝트
        public InventoryRenewalUI_ListEntity itemListEntity; // 프리팹오브젝트 

        private void Awake()
        {
            itemListEntity.gameObject.SetActive(false);
        }

        public override void Show()
        {
            base.Show();

            // TODO : UserData에 있는 Player Item 정보를 토대로, UI를 갱신한다.
            for(int i=0; i < UserDataModel.Singleton.PlayerItemData.itemDataContainer.Count; i++)
            {
                string itemId = UserDataModel.Singleton.PlayerItemData.itemDataContainer[i].itemID;
                int count = UserDataModel.Singleton.PlayerItemData.itemDataContainer[i].quantity;
                AddItem(itemId, count);
            }
        }


        public void AddItem(string itemId, int count)
        {
            //TODO : UI상에, itemListEntity를 복제해서 추가..
            var itemDataSO = GameDataModel.Singleton.ItemData.GetItemDataSO(itemId);
            InventoryRenewalUI_ListEntity newItemEntity = Instantiate(itemListEntity, listRoot);
            newItemEntity.gameObject.SetActive(true);
            newItemEntity.Init(itemDataSO.ItemIcon, itemDataSO.ItemName, count);
            
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
