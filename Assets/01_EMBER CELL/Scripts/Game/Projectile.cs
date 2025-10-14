using UnityEngine;
using UnityEngine.Pool;
namespace TEC
{
    public class Projectile : MonoBehaviour
    {
        private IObjectPool<Projectile> pool;
        private float lifetime = 3f;
        private float lifeTimer;

        private Rigidbody rb;
        private bool initialized = false;

        private GameObject attacker;
        private float damage;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Initialize(GameObject attacker, float damage)
        {
            this.attacker = attacker;
            this.damage = damage;
            lifeTimer = lifetime;
            initialized = true;

            rb.velocity = transform.forward * 100f;
        }

        public void SetPool(IObjectPool<Projectile> pool)
        {
            this.pool = pool;
        }

        private void Update()
        {
            if (!initialized) return;

            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                ReleaseToPool();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.gameObject == attacker)
                return;

            if (collision.transform.TryGetComponent(out IDamageReceiver receiver))
            {
                var data = new DamageData(damage, attacker);
                receiver.ReceiveDamage(data);
            }

            ReleaseToPool();
        }

        private void ReleaseToPool()
        {
            if (pool != null)
            {
                pool.Release(this); 
            }
            else
            {
                gameObject.SetActive(false); 
            }
        }


        private void OnDisable()
        {
            rb.velocity = Vector3.zero;
            initialized = false;
        }
    }
}
