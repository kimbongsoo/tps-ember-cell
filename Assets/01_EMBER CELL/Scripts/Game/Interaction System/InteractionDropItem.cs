using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class InteractionDropItem : MonoBehaviour, IInteractionProvider
    {
        public List<IInteractionData> Interactions => interactionDatas;

        [SerializeField] private InteractionDropItemData itemData;
        [SerializeField] private MeshRenderer visualRenderer;
        [SerializeField] private Material[] gradeMaterials;
        [SerializeField] private string pickupAnimatorTrigger = "Pickup Trigger";

        private readonly List<IInteractionData> interactionDatas = new();

        private void Awake()
        {
            if (itemData != null)
                interactionDatas.Add(itemData);

            if (visualRenderer != null && gradeMaterials != null && gradeMaterials.Length > 0 && itemData != null)
            {
                int idx = Mathf.Clamp(itemData.ItemGrade - 1, 0, gradeMaterials.Length - 1);
                visualRenderer.material = gradeMaterials[idx];
            }
        }

        public void Interact(IInteractionData data)
        {
            if (data is not InteractionDropItemData) return;

            var character = FindObjectOfType<CharacterBase>();
            if (character == null) return;

            // 줍는 모션
            character.characterAnimator?.SetTrigger(pickupAnimatorTrigger);

            // 탄약 증가
            TryAddAmmoToCharacter(character, 15);

            // 잠깐 대기 후 아이템 삭제
            StartCoroutine(DestroyAfterDelay());
        }

        private void TryAddAmmoToCharacter(CharacterBase character, int amount)
        {
            if (character.PrimaryWeapon == null) return;

            character.PrimaryWeapon.AddAmmo(amount, out int current, out int max);
            character.onReloadCompleteEvent?.Invoke(current, max);
        }

        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(0.25f);
            Destroy(gameObject);
        }
    }
}
