using UnityEngine;

namespace TEC
{
    /// <summary>
    /// 주변 상호작용 가능 오브젝트 감지 + UI 갱신
    /// </summary>
    public class InteractionSensor : MonoBehaviour
    {
        [SerializeField] private float sensorRadius = 2f;
        private Collider[] overlappedByPulse = new Collider[32];

        private void Awake()
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            var col = gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = sensorRadius;
        }

        public void PulseManually()
        {
            var ui = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
            ui.ClearData();

            int count = Physics.OverlapSphereNonAlloc(transform.position, sensorRadius, overlappedByPulse);
            for (int i = 0; i < count; i++)
            {
                if (overlappedByPulse[i].TryGetComponent<IInteractionProvider>(out var provider))
                {
                    foreach (var data in provider.Interactions)
                        ui.AddInteractionData(new InteractionDataContext(data, provider));
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IInteractionProvider>(out var provider))
            {
                var ui = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
                foreach (var data in provider.Interactions)
                    ui.AddInteractionData(new InteractionDataContext(data, provider));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IInteractionProvider>(out var provider))
            {
                var ui = UIManager.Singleton.GetUI<InteractionUI>(UIList.InteractionUI);
                foreach (var data in provider.Interactions)
                    ui.RemoveInteractionData(new InteractionDataContext(data, provider));
            }
        }
    }
}
