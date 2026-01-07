using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class InteractionDropItem : MonoBehaviour, IInteractionProvider
    {
        public List<IInteractionData> Interactions => interactionDatas;

        [SerializeField] private Material[] itemGradeMaterials = new Material[5];
        [SerializeField] private MeshRenderer visualRenderer;

        private List<IInteractionData> interactionDatas = new();

        public void Initialize(InteractionDropItemData itemData)
        {
            interactionDatas.Add(itemData);

            int index = Mathf.Clamp(itemData.ItemGrade - 1, 0, itemGradeMaterials.Length - 1);
            visualRenderer.material = itemGradeMaterials[index];
        }

        public void Interact(IInteractionData data)
        {   
            //TODO : 아이템 획득 처리
            //TODO : 인벤토리 추가
            // CharacterPlayerController.Instance?.InteractionSensor?.PulseManually();
            CharacterPlayerController.Instance?.InteractionSensor?.PulseManuallyNextFrame();
            Destroy(gameObject);
        }

    }
}
