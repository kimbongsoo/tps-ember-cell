using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class InteractionDropItem : MonoBehaviour, IInteractionProvider
    {
        public List<IInteractionData> Interactions => interactionDatas;

        // [SerializeField] private Material[] itemGradeMaterials = new Material[5];
        [SerializeField] private MeshRenderer visualRenderer;
        

        private List<IInteractionData> interactionDatas = new();

        // public void Initialize(InteractionDropItemData itemData)
        public void Initialize(ItemDataSO itemData)
        {
            interactionDatas.Add(itemData);

            // int index = Mathf.Clamp(itemData.ItemGrade - 1, 0, itemGradeMaterials.Length - 1);
            // visualRenderer.material = itemGradeMaterials[index];
        }

        // 0119
        public void Interact(IInteractionData data)
        {
            // [CHANGED] ItemDataSO로 캐스팅해서 인벤토리에 추가한다.
            if (data is not ItemDataSO itemDataSO)
                return;

            // [CHANGED] DropQuantity 만큼 인벤에 추가
            bool added = UserDataModel.Singleton.AddItem(itemDataSO.ItemID, itemDataSO.DropQuantity);
            if (!added)
                return;

            // [CHANGED] Interaction UI 갱신(다음 프레임에 재탐색)
            CharacterPlayerController.Instance?.InteractionSensor?.PulseManuallyNextFrame();
            //TODO : 아이템 획득 처리
            //TODO : 인벤토리에 추가

            Destroy(gameObject);
        }
    }
}
