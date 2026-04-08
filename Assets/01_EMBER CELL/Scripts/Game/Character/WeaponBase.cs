using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Pool;
using UnityEngine.TextCore.Text;

namespace TEC
{
    public class WeaponBase : MonoBehaviour
    {
        public int RemainAmmo => clipAmmo;
        public int MaxAmmo => GetReserveAmmoFromInventory();
        public int MaxClipAmmo => maxClipAmmo;

        [Header("Ammo Setting")]
        [SerializeField] private string ammoItemID = "";
        public string AmmoItemID => ammoItemID;

        public CharacterBase Owner { get; set; }

        [Header("Fire Setting")]
        [SerializeField] private Transform fireStartPoint;
        [SerializeField] private GameObject originalBullet;
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float lastFireTime = 0f;

        [SerializeField] private int reserveAmmo = 90;

        [SerializeField] private int maxClipAmmo = 30; // 탄창 용량
        [SerializeField] private int clipAmmo = 30;    // 현재 탄창 수

        [Header("Fire Setting")]
        [SerializeField] private float damage = 30f;

        private WeaponRecoil weaponRecoil;
        private IObjectPool<Projectile> projectilePool;
        private CharacterBase ownerCharacter;

        private void Awake()
        {
            weaponRecoil = GetComponent<WeaponRecoil>();

            projectilePool = new ObjectPool<Projectile>(
                CreateProjectile,
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyPooledObject,
                true,
                30,
                100
            ); //10, 50
        }

        public void Initialize(CharacterBase owner)
        {
            ownerCharacter = owner;

        }
    
        //지워
        // public void InitializeReserveAmmoToInventory()
        // {
        //     if (string.IsNullOrEmpty(ammoItemID) || UserDataModel.Singleton == null)
        //         return;

        //     if (reserveAmmo <= 0)
        //         return;

        //     UserDataModel.Singleton.AddItem(ammoItemID, reserveAmmo);
        //     reserveAmmo = 0;
        // }

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
            bool isShootable = clipAmmo > 0 && Time.time >= lastFireTime + fireRate;

            if (isShootable)
            {
                var projectile = projectilePool.Get();
                // projectile.transform.position = fireStartPoint.position;
                // projectile.transform.forward = fireStartPoint.forward;
                projectile.Initialize(Owner.gameObject, damage);

                if(ownerCharacter.IsPlayerCharacter)
                {
                    weaponRecoil?.GenerateRecoil();
                }

                clipAmmo--;

                EffectManager.Instance.SpawnMuzzleEffect(fireStartPoint);

                // [추가] 발사가 성공한 시점에 총 소리 재생
                SoundManager.Singleton.PlaySound("gun_rifle_shot_01", fireStartPoint.position);
                
                lastFireTime = Time.time;
                // Debug.DrawLine(fireStartPoint.position, fireStartPoint.position + fireStartPoint * 10f, Color.yellow, 1f);
            }

            remain = clipAmmo;
            max = GetReserveAmmoFromInventory();

            return isShootable;
        }

        public int SetFullAmmo()
        {
            if (clipAmmo >= maxClipAmmo)
                return clipAmmo;

            if (string.IsNullOrEmpty(ammoItemID) || UserDataModel.Singleton == null)
                return clipAmmo;

            int need = maxClipAmmo - clipAmmo;
            if (need <= 0)
                return clipAmmo;

            int pulled = UserDataModel.Singleton.ConsumeItem(ammoItemID, need);
            if (pulled <= 0)
                return clipAmmo;

            clipAmmo += pulled;
            return clipAmmo;
        }

        public bool IsEmpty()
        {
            return clipAmmo <= 0 && GetReserveAmmoFromInventory() <= 0;
        }

        public void AddAmmo(int amount, out int current, out int max)
        {
            if (!string.IsNullOrEmpty(ammoItemID) && UserDataModel.Singleton != null && amount > 0)
            {
                UserDataModel.Singleton.AddItem(ammoItemID, amount);
            }

            current = clipAmmo;
            max = GetReserveAmmoFromInventory();
        }

        private int GetReserveAmmoFromInventory()
        {
            if (string.IsNullOrEmpty(ammoItemID) || UserDataModel.Singleton == null)
                return 0;

            return UserDataModel.Singleton.GetTotalItemCount(ammoItemID);
        }
    }
}

