using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Pool;

namespace TEC
{
    public class WeaponBase : MonoBehaviour
    {
        // public int RemainAmmo => clipSize;
        // public int MaxAmmo => maxAmmo;
        public int RemainAmmo => clipAmmo;
        public int MaxAmmo => reserveAmmo;

        public CharacterBase Owner {get;set;}


        [Header("Fire Setting")]
        [SerializeField] private Transform fireStartPoint;
        [SerializeField] private GameObject originalBullet;
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float lastFireTime = 0f;
        // [SerializeField] private int maxAmmo = 30;
        // [SerializeField] private int clipSize = 30;
        [SerializeField] private int reserveAmmo = 90;
        [SerializeField] private int maxClipAmmo = 30; //탄창 용량
        [SerializeField] private int clipAmmo = 30; //현재 탄창 수

        [Header("Fire Setting")]

        [SerializeField] private float damage = 30f;

        private WeaponRecoil weaponRecoil;
        // private float recoilRate = 2f;
        // private float recoilVertical = 2f;
        // private float recoilHorizontal = 1f;

        private IObjectPool<Projectile> projectilePool;
        private void Awake()
        {
            weaponRecoil = GetComponent<WeaponRecoil>();

            projectilePool = new ObjectPool<Projectile>(
                CreateProjectile,
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyPooledObject,
                true,
                10,
                50
            );
        }

        //생성
        private Projectile CreateProjectile()
        {
            var bulletObj = Instantiate(originalBullet);
            var proj = bulletObj.GetComponent<Projectile>();
            proj.SetPool(projectilePool);
            return proj;
        }

        private void OnGetFromPool(Projectile projectile)
        {
            projectile.gameObject.SetActive(true);
            projectile.transform.SetPositionAndRotation(fireStartPoint.position, fireStartPoint.rotation);
        }

        private void OnReleaseToPool(Projectile projectile)
        {
            projectile.gameObject.SetActive(false);
        }

        private void OnDestroyPooledObject(Projectile projectile)
        {
            Destroy(projectile.gameObject);
        }



        public bool Shoot(out int remain, out int max)
        {
            // bool isShootable = clipSize > 0 && Time.time >= lastFireTime + fireRate;
            bool isShootable = clipAmmo > 0 && Time.time >= lastFireTime + fireRate;

            if (isShootable)
            {
                var projectile = projectilePool.Get();

                projectile.Initialize(Owner.gameObject, damage);

                weaponRecoil?.GenerateRecoil();

                
                // clipSize--;
                clipAmmo--;

                EffectManager.Instance.SpawnMuzzleEffect(fireStartPoint);

                // CharacterPlayerController.Instance.CameraRecoil(recoilRate, recoilVertical, recoilHorizontal);

                lastFireTime = Time.time;
            }
            
            // remain = clipSize;
            // max = maxAmmo;
            remain = clipAmmo;
            max = reserveAmmo;

            return isShootable;
        }

        public int SetFullAmmo()
        {
            if (clipAmmo >= maxClipAmmo || reserveAmmo <= 0)
                return clipAmmo;

            int need = maxClipAmmo - clipAmmo;
            int toLoad = Mathf.Min(need, reserveAmmo);

            clipAmmo   += toLoad;
            reserveAmmo -= toLoad;

            return clipAmmo;

            // clipSize = maxAmmo;
            // return maxAmmo;
        }

        public bool IsEmpty()
        {
            return clipAmmo <= 0 && reserveAmmo <= 0;
            // return clipSize == 0;
        }

        public void AddAmmo(int amount, out int current, out int max)
        {
            reserveAmmo = Mathf.Max(0, reserveAmmo + amount);

            current = clipAmmo;
            max     = reserveAmmo;
            // clipSize = Mathf.Clamp(clipSize + amount, 0, maxAmmo);
            // current  = clipSize;
            // max      = maxAmmo;
        }
    }
}
