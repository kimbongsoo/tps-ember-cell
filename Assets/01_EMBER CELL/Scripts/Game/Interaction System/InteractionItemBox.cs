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
                boxData.DropItems.ForEach(dropData =>
                {
                    var newDropItem = Instantiate(dropItemPrefab);
                    newDropItem.Initialize(dropData);
                    newDropItem.transform.SetPositionAndRotation(spawnLocation.position, spawnLocation.rotation);

                    Vector3 forward = spawnLocation.forward;

                    // 중심축에서 좌우로 퍼지는 각도 설정
                    float anngleH = UnityEngine.Random.Range(-60f, 60f);
                    float anngleV = UnityEngine.Random.Range(45f, 60f);

                    // 각도를 방향 벡터로 변환
                    Quaternion rotationH = Quaternion.AngleAxis(anngleH, Vector3.up);
                    Quaternion rotationV = Quaternion.AngleAxis(-anngleV, spawnLocation.right);
                    Vector3 direction = rotationH * rotationV * forward;

                    Vector3 spawnPos = spawnLocation.position + Vector3.up * 0.2f;
                    if (newDropItem.TryGetComponent<Rigidbody>(out var rb))
                    {
                        float force = UnityEngine.Random.Range(30, 45f); //포물선 탄도에 적당 초기 속도
                        rb.AddForce(direction.normalized * force, ForceMode.Impulse);
                    }
                });

                //상자를 파괴한다.
                DestroyImmediate(gameObject);

                // 수동으로 Interaction Sensor를 1회 작동 시킨다.
                CharacterPlayerController.Instance.InteractionSensor.PulseManually();


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
