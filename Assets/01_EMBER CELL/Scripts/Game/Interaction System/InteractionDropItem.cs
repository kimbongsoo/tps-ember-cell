using System.Collections;
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
        //원본
        // public void Interact(IInteractionData data)
        // {
        //     //TODO : 아이템 획득 처리
        //     //TODO : 인벤토리에 추가
        //     if (data is InteractionDropItemData itemData)
        //     {
        //         var playerController = CharacterPlayerController.Instance;
        //         if (playerController != null)
        //         {
        //             var character = playerController.GetComponent<CharacterBase>();
        //             if (character != null && character.PrimaryWeapon != null)
        //             {
        //                 character.PrimaryWeapon.AddAmmo(itemData.AmmoAmount, out int current, out int max);
        //                 MainHUD.Instance.SetAmmoText(current, max);
        //             }
        //         }
        //     }

        //     Destroy(gameObject);
        // }
        public void Interact(IInteractionData data)
        {
            if (data is InteractionDropItemData itemData)
            {
                if (PlayerInventory.Instance != null)
                {
                    bool added = PlayerInventory.Instance.TryAddItem(itemData, 1);
                    if (added)
                    {
                        Destroy(gameObject);
                        return;
                    }
                }
            }
        }
    }
}
