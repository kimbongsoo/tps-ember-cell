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

        private bool isReleasedToPool = false;

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

            isReleasedToPool = false;

            rb.velocity = transform.forward * 100f;
        }

        public void SetPool(IObjectPool<Projectile> pool)
        {
            this.pool = pool;
        }

        private void Update()
        {
            if (!initialized || isReleasedToPool) 
                return;

            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                ReleaseToPool();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!initialized || isReleasedToPool)
                return;

            if (collision.transform.gameObject == attacker)
                return;
            //
            // if (collision.transform.TryGetComponent(out IDamageReceiver receiver))
            // {
            //     var data = new DamageData(damage, attacker);
            //     receiver.ReceiveDamage(data);
            // }
            var receiver = collision.transform.GetComponentInParent<IDamageReceiver>();
            if (receiver != null)
            {
                var data = new DamageData(damage, attacker);
                receiver.ReceiveDamage(data);
            }

            var physMat = collision.collider.sharedMaterial;
            EffectManager.Instance.SpawnImpactEffect(collision.contacts[0], physMat);

            ReleaseToPool();
        }

        private void ReleaseToPool()
        {
            if (isReleasedToPool)
                return;

            isReleasedToPool = true;
            initialized = false;

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
