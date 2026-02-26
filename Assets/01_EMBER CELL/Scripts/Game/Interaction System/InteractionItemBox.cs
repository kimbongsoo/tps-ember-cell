using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

namespace TEC
{
    public class InteractionItemBox : MonoBehaviour, IInteractionProvider
    {
        public List<IInteractionData> Interactions => new List<IInteractionData>()
        {
            searchData,
            boxData,
            
        };

        [SerializeField] private InteractionSearchData searchData;
        [SerializeField] private InteractionItemBoxData boxData;
        [SerializeField] private Transform spawnLocation;
        [SerializeField] private GameObject searchCamera;

        public void Interact(IInteractionData data)
        {
            if (data is InteractionItemBoxData)
            {
                //TODO : Drop Item을 드롭한다.
                var itemBoxData = data as InteractionItemBoxData;
                var dropItemPrefab = Resources.Load<InteractionDropItem>("Interaction Prefabs/Interaction Drop Item");

                // Destroy 이후 spawnLocation 접근하면 MissingReferenceException이 날 수 있으니
                // 파괴 전에 위치/회전/방향 캐싱
                Vector3 cachedPos = spawnLocation.position;
                Quaternion cachedRot = spawnLocation.rotation;
                Vector3 cachedForward = spawnLocation.forward;
                Vector3 cachedRight = spawnLocation.right;

                itemBoxData.DropItems.ForEach(dropData =>
                {
                    var newDropItem = Instantiate(dropItemPrefab);
                    newDropItem.Initialize(dropData);
                    newDropItem.transform.SetPositionAndRotation(cachedPos, cachedRot);

                    Vector3 forward = cachedForward;

                    // 중심축에서 좌우로 퍼지는 각도 설정
                    float anngleH = UnityEngine.Random.Range(-18f, 18f); //원래 60
                    float anngleV = UnityEngine.Random.Range(18f, 28f); // 45 60

                    // 각도를 방향 벡터로 변환
                    Quaternion rotationH = Quaternion.AngleAxis(anngleH, Vector3.up);
                    Quaternion rotationV = Quaternion.AngleAxis(-anngleV, cachedRight);
                    // Vector3 direction = rotationH * rotationV * forward; 추가
                    Vector3 direction = rotationH * rotationV * forward;
                    direction = Vector3.Lerp(direction, Vector3.up, 0.25f);

                    Vector3 spawnPos = cachedPos + Vector3.up * 0.2f;
                    newDropItem.transform.position = spawnPos;

                    if (newDropItem.TryGetComponent<Rigidbody>(out var rb))
                    {
                        float force = UnityEngine.Random.Range(6, 9f); //포물선 탄도에 적당 초기 속도 30, 45
                        rb.AddForce(direction.normalized * force, ForceMode.Impulse);
                    }
                });

                // TODO : Interaction UI를 갱신
                //추가
                CharacterPlayerController.Instance?.InteractionSensor?.PulseManuallyNextFrame();
                Destroy(gameObject);
                return;
            }
            else if (data is InteractionSearchData)
            {
                // 1. Box에 연결 되어있는 Virtual Camera를 활성화한다.
                // 2. 3초 뒤에 Virtual Camera를 비활성화

                searchCamera.SetActive(true);
                StartCoroutine(DelayedSearchCameraInactive());
            }

            IEnumerator DelayedSearchCameraInactive()
            {
                yield return new WaitForSeconds(3f);
                searchCamera.SetActive(false);
            }

            // TODO : Interaction UI를 갱신
        }
    }
}
