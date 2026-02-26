using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace TEC
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get; private set; }

        [Header("Impact Effects")]
        [SerializeField] private GameObject woodImpactPrefab;
        [SerializeField] private GameObject metalImpactPrefab;
        [SerializeField] private GameObject rockImpactPrefab;
        [SerializeField] private GameObject dirtImpactPrefab;
        [SerializeField] private GameObject defaultImpactPrefab;

        [Header("Weapon Effects")]
        [SerializeField] private GameObject muzzleEffectPrefab;

        private Dictionary<string, IObjectPool<GameObject>> impactPools;
        private IObjectPool<GameObject> muzzlePool;
        private void Awake()
        {
            Instance = this;

            impactPools = new Dictionary<string, IObjectPool<GameObject>>
            {
                { "Wood", CreatePool(woodImpactPrefab) },
                { "Metal", CreatePool(metalImpactPrefab) },
                { "Rock", CreatePool(rockImpactPrefab) },
                { "Dirt", CreatePool(dirtImpactPrefab) },
                { "Default", CreatePool(defaultImpactPrefab) }
            };

            muzzlePool = CreatePool(muzzleEffectPrefab);
        }

        private IObjectPool<GameObject> CreatePool(GameObject prefab)
        {
            return new ObjectPool<GameObject>(
                () => Instantiate(prefab),
                effect => effect.SetActive(true),
                effect => effect.SetActive(false),
                effect => Destroy(effect),
                true, 50, 100
            );
        }

        public void SpawnMuzzleEffect(Transform firePoint)
        {
            var muzzle = muzzlePool.Get();
            muzzle.transform.position = firePoint.position;
            muzzle.transform.rotation = firePoint.rotation;
            StartCoroutine(ReleaseAfterDelay(muzzle, muzzlePool, 1f));
        }

        public void SpawnImpactEffect(ContactPoint contact, PhysicMaterial material)
        {
            string matType = GetMaterialType(material);
            var pool = impactPools.ContainsKey(matType) ? impactPools[matType] : impactPools["Default"];

            var effect = pool.Get();
            effect.transform.position = contact.point + contact.normal * 0.01f;
            effect.transform.rotation = Quaternion.LookRotation(contact.normal);

            StartCoroutine(ReleaseAfterDelay(effect, pool, 2f));
        }

        private string GetMaterialType(PhysicMaterial mat)
        {
            if (mat == null) return "Default";
            string name = mat.name;
            if (name.Contains("Wood")) return "Wood";
            if (name.Contains("Metal")) return "Metal";
            if (name.Contains("Rock")) return "Rock";
            if (name.Contains("Dirt")) return "Dirt";
            return "Default";
        }

        private System.Collections.IEnumerator ReleaseAfterDelay(GameObject effect, IObjectPool<GameObject> pool, float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.Release(effect);
        }
    }
}
