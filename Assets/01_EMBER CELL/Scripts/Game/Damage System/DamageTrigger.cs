using UnityEngine;

namespace TEC
{
    [RequireComponent(typeof(Collider))]
    public class DamageTrigger : MonoBehaviour
    {
        [SerializeField] private float damageAmount = 100f;
        [SerializeField] private bool destroyAfterHit = true; 

        private void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var receiver = other.GetComponentInParent<CharacterBase>();
            if (receiver == null)
                return;

            var damageData = new DamageData(damageAmount, gameObject);
            receiver.ReceiveDamage(damageData);

            Debug.Log($"{receiver.name}에게 {damageAmount} 데미지 전달됨");

            if (destroyAfterHit)
                Destroy(gameObject);
        }
    }
}
