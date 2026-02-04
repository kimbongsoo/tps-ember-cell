using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class InteractionDropItem : MonoBehaviour, IInteractionProvider
    {
        public List<IInteractionData> Interactions => interactionDatas;

        [SerializeField] private MeshRenderer visualRenderer;
        

        private List<IInteractionData> interactionDatas = new();

        public void Initialize(ItemDataSO itemData)
        {
            interactionDatas.Add(itemData);

        }

        // 0119
        public void Interact(IInteractionData data)
        {
            if (data is not ItemDataSO itemDataSO)
                return;

            bool added = UserDataModel.Singleton.AddItem(itemDataSO.ItemID, itemDataSO.DropQuantity);
            if (!added)
                return;

            CharacterPlayerController.Instance.InteractionSensor.PulseManuallyNextFrame();
            //TODO : 아이템 획득 처리
            //TODO : 인벤토리에 추가

            Destroy(gameObject);
        }
    }
}
