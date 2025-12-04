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

        public void Interact(IInteractionData data)
        {
            //TODO : 아이템 획득 처리
            //TODO : 인벤토리에 추가
                // 드랍 아이템 데이터인지 확인
            if (data is InteractionDropItemData itemData)
            {
                // 1. 플레이어 컨트롤러 찾기
                var playerController = CharacterPlayerController.Instance;
                if (playerController != null)
                {
                    // 2. 플레이어 캐릭터/무기 가져오기
                    var character = playerController.GetComponent<CharacterBase>();
                    if (character != null && character.PrimaryWeapon != null)
                    {
                        // 3. 무기에 탄약 추가
                        character.PrimaryWeapon.AddAmmo(itemData.AmmoAmount, out int current, out int max);

                        // 4. HUD 갱신
                        MainHUD.Instance.SetAmmoText(current, max);

                        Debug.Log($"[DropItem] Ammo +{itemData.AmmoAmount} → {current}/{max}");
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}
