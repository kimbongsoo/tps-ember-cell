using UnityEngine;

namespace TEC
{
    [RequireComponent(typeof(Collider))]
    public class DamageTrigger : MonoBehaviour
    {
        [SerializeField] private float damageAmount = 100f;
        [SerializeField] private bool destroyAfterHit = true; // ✅ 기본값 true로 설정

        private void Reset()
        {
            // ✅ Trigger 충돌을 위해 isTrigger 자동 활성화
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            // ✅ CharacterBase를 부모에서라도 찾기 (자식 collider 포함)
            var receiver = other.GetComponentInParent<CharacterBase>();
            if (receiver == null)
                return;

            // ✅ 데미지 전달
            var damageData = new DamageData(damageAmount, gameObject);
            receiver.ReceiveDamage(damageData);

            Debug.Log($"{receiver.name}에게 {damageAmount} 데미지 전달됨");

            // ✅ 트리거 오브젝트 파괴
            if (destroyAfterHit)
                Destroy(gameObject);
        }
    }
}
