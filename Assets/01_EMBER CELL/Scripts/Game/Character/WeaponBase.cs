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
        public int RemainAmmo => clipSize;
        public int MaxAmmo => maxAmmo;

        public CharacterBase Owner {get;set;}


        [Header("Fire Setting")]
        [SerializeField] private Transform fireStartPoint;
        [SerializeField] private GameObject originalBullet;
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float lastFireTime = 0f;
        [SerializeField] private int maxAmmo = 30;
        [SerializeField] private int clipSize = 30;

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
            bool isShootable = clipSize > 0 && Time.time >= lastFireTime + fireRate;
            if (isShootable)
            {
                var projectile = projectilePool.Get();

                projectile.Initialize(Owner.gameObject, damage);

                weaponRecoil?.GenerateRecoil();

                

                clipSize--;

                if (EffectManager.Instance.GetEffect("Muzzle", out GameObject muzzleEffect))
                {
                    muzzleEffect.transform.position = fireStartPoint.position;
                    muzzleEffect.transform.rotation = fireStartPoint.rotation;
                    Destroy(muzzleEffect.gameObject, 1f);
                }

                // CharacterPlayerController.Instance.CameraRecoil(recoilRate, recoilVertical, recoilHorizontal);

                lastFireTime = Time.time;
            }
            remain = clipSize;
            max = maxAmmo;

            return isShootable;
        }

        public int SetFullAmmo()
        {
            clipSize = maxAmmo;
            return maxAmmo;
        }

        public bool IsEmpty()
        {
            return clipSize == 0;
        }
    }
}
