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
        public int MaxAmmo => GetReserveAmmo(); // [CHANGED]
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

        [SerializeField] private int maxClipAmmo = 30;
        [SerializeField] private int clipAmmo = 30;

        [Header("Fire Setting")]
        [SerializeField] private float damage = 30f;

        private WeaponRecoil weaponRecoil;
        private IObjectPool<Projectile> projectilePool;
        private CharacterBase ownerCharacter;

        private int pendingReloadAmmo = 0; // [CHANGED]

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
            );
        }

        public void Initialize(CharacterBase owner)
        {
            ownerCharacter = owner;
        }

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
                projectile.Initialize(Owner.gameObject, damage);

                if (ownerCharacter.IsPlayerCharacter)
                {
                    weaponRecoil?.GenerateRecoil();
                }

                clipAmmo--;

                EffectManager.Instance.SpawnMuzzleEffect(fireStartPoint);

                SoundManager.Singleton.PlaySound("gun_rifle_shot_01", fireStartPoint.position);

                lastFireTime = Time.time;
            }

            remain = clipAmmo;
            max = GetReserveAmmo(); 

            return isShootable;
        }

        public void PrepareReloadAmmo(int ammoAmount) 
        {
            if (ammoAmount <= 0)
            {
                pendingReloadAmmo = 0;
                return;
            }

            int need = maxClipAmmo - clipAmmo;
            pendingReloadAmmo = Mathf.Clamp(ammoAmount, 0, need);
        }

        public int SetFullAmmo()
        {
            if (clipAmmo >= maxClipAmmo)
            {
                pendingReloadAmmo = 0; 
                return clipAmmo;
            }

            int need = maxClipAmmo - clipAmmo; 
            if (need <= 0) 
            {
                pendingReloadAmmo = 0; 
                return clipAmmo; 
            }

            if (ownerCharacter != null && ownerCharacter.IsPlayerCharacter) 
            {
                if (pendingReloadAmmo <= 0) 
                    return clipAmmo; 

                int addAmount = Mathf.Min(need, pendingReloadAmmo); 
                clipAmmo += addAmount; 
                pendingReloadAmmo = 0; 
                return clipAmmo; 
            }

            // AI는 내부 reserveAmmo를 사용
            int aiPulled = Mathf.Min(need, reserveAmmo); 
            if (aiPulled <= 0) 
                return clipAmmo; 

            clipAmmo += aiPulled; 
            reserveAmmo -= aiPulled; 

            return clipAmmo;
        }

        public bool IsEmpty()
        {
            return clipAmmo <= 0 && GetReserveAmmo() <= 0; 
        }

        public void AddAmmo(int amount, out int current, out int max)
        {
            if (amount > 0) 
            {
                if (ownerCharacter != null && ownerCharacter.IsPlayerCharacter) 
                {
                    if (!string.IsNullOrEmpty(ammoItemID) && UserDataModel.Singleton != null) 
                    {
                        UserDataModel.Singleton.AddItem(ammoItemID, amount); 
                    }
                }
                else 
                {
                    reserveAmmo += amount; 
                }
            }

            current = clipAmmo;
            max = GetReserveAmmo(); 
        }

        private int GetReserveAmmo() 
        {
            if (ownerCharacter != null && ownerCharacter.IsPlayerCharacter) 
            {
                if (string.IsNullOrEmpty(ammoItemID) || UserDataModel.Singleton == null) 
                    return 0; 

                return UserDataModel.Singleton.GetTotalItemCount(ammoItemID); 
            }

            return reserveAmmo; 
        }
    }
}