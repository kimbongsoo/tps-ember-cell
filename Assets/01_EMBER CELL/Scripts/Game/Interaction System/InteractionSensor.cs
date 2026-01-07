using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TEC
{
    public class InteractionSensor : MonoBehaviour
    {
        [SerializeField] private float sensorRadius = 2f;
        private Rigidbody sensorRigidbody;
        private SphereCollider sensorCollider;

        private Collider[] overlappedByPulse = new Collider[32];


        void Awake()
        {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            sensorRigidbody = gameObject.AddComponent<Rigidbody>();
            sensorRigidbody.isKinematic = true;

            sensorCollider = gameObject.AddComponent<SphereCollider>();
            sensorCollider.radius = sensorRadius;
            sensorCollider.isTrigger = true;

            //InteractionUI 숨김처리
            UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI)?.Hide();
        }

        // - Destroy된 오브젝트가 씬에서 완전히 제거된 다음에 갱신
        public void PulseManuallyNextFrame()
        {
            StartCoroutine(PulseManuallyNextFrameRoutine());
        }

        private IEnumerator PulseManuallyNextFrameRoutine()
        {
            yield return null; // 다음 프레임
            PulseManually();
        }

        // 수동으로 센서 기능을 한번 실행해보는 함수
        public void PulseManually()
        {
            var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            interactionUI.ClearData();

            int overlappedCount = Physics.OverlapSphereNonAlloc(transform.position, sensorRadius, overlappedByPulse);
            for (int i = 0; i < overlappedCount; i++)
            {
                if (overlappedByPulse[i].TryGetComponent<IInteractionProvider>(out var provider))
                {
                    foreach (var data in provider.Interactions)
                    {
                        var context = new InteractionDataContext(data, provider);
                        interactionUI.AddInteractionData(context);
                    }
                }
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            IInteractionProvider interactionProvider = other.GetComponent<IInteractionProvider>();
            if (interactionProvider != null)
            {
                var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
                foreach (var data in interactionProvider.Interactions)
                {
                    var context = new InteractionDataContext(data, interactionProvider);
                    interactionUI.AddInteractionData(context);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            IInteractionProvider interactionProvider = other.GetComponent<IInteractionProvider>();
            if (interactionProvider != null)
            {
                var interactionUI = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
                foreach (var data in interactionProvider.Interactions)
                {
                    var context = new InteractionDataContext(data, interactionProvider);
                    interactionUI.RemoveInteractionData(context);
                }
            } 
        }
    }
}
