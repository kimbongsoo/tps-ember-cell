using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TEC
{
    public class WeaponBase : MonoBehaviour, IDamageAttacker
    {
        public int RemainAmmo => clipSize;
        public int MaxAmmo => maxAmmo;
        public IDamageData CreateDamageData(float baseDamage, GameObject attacker)
        {
            return new DamageData(baseDamage, attacker);
        }

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
        private float recoilRate = 2f;
        private float recoilVertical = 2f;
        private float recoilHorizontal = 1f;
        private void Awake()
        {
            weaponRecoil = GetComponent<WeaponRecoil>();
        }

        public bool Shoot(out int remain, out int max)
        {
            bool isShootable = clipSize > 0 && Time.time >= lastFireTime + fireRate;
            if (isShootable)
            {
                GameObject bullet = Instantiate(originalBullet, fireStartPoint.position, fireStartPoint.rotation);
                bullet.SetActive(true);


                var projectile = bullet.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.Initialize(CharacterPlayerController.Instance.gameObject, damage); 
                }

                //반동
                weaponRecoil?.GenerateRecoil();

                clipSize--;

                if (EffectManager.Instance.GetEffect("Muzzle", out GameObject muzzleEffect))
                {
                    muzzleEffect.transform.position = fireStartPoint.position;
                    muzzleEffect.transform.rotation = fireStartPoint.rotation;
                    Destroy(muzzleEffect.gameObject, 1f);
                }

                CharacterPlayerController.Instance.CameraRecoil(recoilRate, recoilVertical, recoilHorizontal);

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
