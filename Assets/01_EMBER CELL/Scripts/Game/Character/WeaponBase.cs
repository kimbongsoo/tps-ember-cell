using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TEC
{
    public class WeaponBase : MonoBehaviour
    {
        public int RemainAmmo => clipSize;
        public int MaxAmmo => maxAmmo;

        [Header("Fire Setting")]
        [SerializeField] private Transform fireStartPoint;
        [SerializeField] private GameObject originalBullet;
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float lastFireTime = 0f;
        [SerializeField] private int maxAmmo = 30;
        [SerializeField] private int clipSize = 30;

        [Header("Fire Setting")]

        [SerializeField] private float damage = 30f;

        private CharacterBase owner;

        private void Awake()
        {
            owner = GetComponentInParent<CharacterBase>();
        }

        public bool Shoot(out int remain, out int max)
        {
            bool isShootable = clipSize > 0 && Time.time >= lastFireTime + fireRate;
            if (isShootable)
            {
                GameObject bullet = Instantiate(originalBullet, fireStartPoint.position, fireStartPoint.rotation);
                //방금 추가
                Debug.DrawRay(fireStartPoint.position, fireStartPoint.forward * 10f, Color.red, 1f);
                bullet.SetActive(true);

                // bullet.SetActive(true); 다음 줄 쯤에
                Debug.Log($"[WeaponBase] Bullet spawned: {bullet.name} at {fireStartPoint.position}");


                //추가
                if (bullet.TryGetComponent(out Projectile projectile))
                {
                    projectile.Initialize(damage, owner ? owner.gameObject : gameObject);
                }

                clipSize--;

                if (EffectManager.Instance.GetEffect("Muzzle", out GameObject muzzleEffect))
                {
                    muzzleEffect.transform.position = fireStartPoint.position;
                    muzzleEffect.transform.rotation = fireStartPoint.rotation;
                    Destroy(muzzleEffect.gameObject, 1f);
                }

                float recoil = 2f;
                float vertical = 2f;
                float horizontal = 1f;
                CharacterPlayerController.Instance.CameraRecoil(recoil, vertical, horizontal);

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
