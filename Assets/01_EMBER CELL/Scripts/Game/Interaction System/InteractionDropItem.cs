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
            
            //추가
            ApplyWorldVisual(itemData);
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

        private void ApplyWorldVisual(ItemDataSO itemData)
        {
            if (itemData == null)
                return;

            if (visualRenderer == null)
                return;

            if (itemData.WorldPrefab == null)
                return;

            var srcRenderer = itemData.WorldPrefab.GetComponentInChildren<MeshRenderer>(true);
            var srcFilter = itemData.WorldPrefab.GetComponentInChildren<MeshFilter>(true);

            if (srcRenderer == null || srcFilter == null)
                return;

            var dstFilter = visualRenderer.GetComponent<MeshFilter>();
            if (dstFilter == null)
                return;

            dstFilter.sharedMesh = srcFilter.sharedMesh;
            visualRenderer.sharedMaterials = srcRenderer.sharedMaterials;

            if (TryGetComponent<MeshCollider>(out var dstCollider))
            {
                dstCollider.sharedMesh = srcFilter.sharedMesh;
            }
        }
    }
}
