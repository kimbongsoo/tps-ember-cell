using UnityEngine;

namespace TEC
{
    public class DamageableCube : MonoBehaviour, IDamageReceiver
    {
        [SerializeField] private float maxHP = 50f;
        private float currentHP;

        public float CurrentHP => currentHP;
        public float MaxHP => maxHP;

        private void Start()
        {
            currentHP = maxHP;
            Debug.Log($"[CubeTarget] HP Initialized: {currentHP}/{maxHP}");
        }

        public void ReceiveDamage(IDamageData damageData)
        {
            currentHP -= damageData.DamageAmount;
            Debug.Log($"[CubeTarget] Hit by {damageData.Attacker.name}, Damage: {damageData.DamageAmount}, HP: {currentHP}/{maxHP}");

            if (currentHP <= 0)
            {
                Debug.Log("[CubeTarget] Destroyed!");
                Destroy(gameObject);
            }
        }
    }
}
