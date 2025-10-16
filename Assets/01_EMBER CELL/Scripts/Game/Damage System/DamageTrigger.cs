using UnityEngine;

namespace TEC
{
    [RequireComponent(typeof(Collider))]
    public class DamageTrigger : MonoBehaviour
    {
        [SerializeField] private float damageAmount = 100f;
        [SerializeField] private bool destroyAfterHit = false;

        private void OnTriggerEnter(Collider other)
        {
            var receiver = other.GetComponent<CharacterBase>();
            if (receiver != null)
            {
                receiver.ReceiveDamage(new DamageData(damageAmount, gameObject));

                if (destroyAfterHit)
                    Destroy(gameObject);
            }
        }
    }
}
