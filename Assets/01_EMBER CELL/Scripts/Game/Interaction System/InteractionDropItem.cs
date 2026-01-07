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

        // public void Interact(IInteractionData data)
        // {
        //     CharacterPlayerController.Instance?.InteractionSensor?.PulseManuallyNextFrame();
        //     Destroy(gameObject);
        // }
        public void Interact(IInteractionData data)
        {
            Debug.Log($"[InteractionDropItem] Interact() data={(data == null ? "null" : data.GetType().Name)}", this);

            if (data is InteractionDropItemData dropData)
            {
                Debug.Log($"[InteractionDropItem] Pick item={dropData.name} Icon={(dropData.ActionIcon == null ? "null" : dropData.ActionIcon.name)}", this);

                bool added = Inventory.Singleton != null && Inventory.Singleton.AddItem(dropData);
                Debug.Log($"[InteractionDropItem] Inventory.AddItem => {added}", this);
            }

            CharacterPlayerController.Instance?.InteractionSensor?.PulseManuallyNextFrame();
            Destroy(gameObject);
        }

    }
}
