using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

// 파일 상단 using에 이미 있으니 유지
// using System.Linq;
// using UnityEngine;

namespace TEC
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 100f;
        public float lifeTime = 3f;
        public static string[] materialNames = { "Wood", "Rock", "Dirt", "Metal" };

        // ↓↓↓ 이미 네가 추가한 필드가 있다면 유지하고, 없다면 아래처럼 두어도 OK
        private float damage = 0f;
        private GameObject attacker;
        private Collider selfCollider;

        private void Awake()
        {
            selfCollider = GetComponent<Collider>();

            // ★ 필수: 터널링 방지 & 물리 보간
            var rb = GetComponent<Rigidbody>();
            if (rb)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }

            // ★ 필수: 실수로 Trigger 켜져 있으면 충돌 안 들어옴
            if (selfCollider && selfCollider.isTrigger)
            {
                Debug.LogWarning($"[Projectile] Collider.isTrigger 가 켜져 있어 껐습니다. ({name})");
                selfCollider.isTrigger = false;
            }
        }

        // WeaponBase에서 호출하도록 만든 초기화 (네가 3번에서 붙인 그대로)
        public void Initialize(float newDamage, GameObject newAttacker)
        {
            damage = newDamage;
            attacker = newAttacker;

            // 발사자와의 충돌 무시
            if (selfCollider && attacker)
            {
                var cols = attacker.GetComponentsInChildren<Collider>();
                for (int i = 0; i < cols.Length; i++)
                    Physics.IgnoreCollision(selfCollider, cols[i], true);
            }

            // 디버그
            Debug.Log($"[Projectile] Initialize dmg={damage}, attacker={(attacker ? attacker.name : "null")} ({name})");
        }

        private void Start()
        {
            var rb = GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddForce(transform.forward * speed, ForceMode.Impulse);
            }
            else
            {
                Debug.LogError($"[Projectile] Rigidbody 가 없습니다. 충돌이 동작하지 않을 수 있습니다. ({name})");
            }

            Debug.Log($"[Projectile] Spawned speed={speed} ({name})");

            Destroy(gameObject, lifeTime);
        }
        private void OnCollisionEnter(Collision collision)
        {
            // 1) 이펙트 처리 (네 기존 로직 유지)
            string material = materialNames.FirstOrDefault(type => collision.collider.material.name.Contains(type));
            if (EffectManager.Instance.GetEffect(material, out GameObject effect))
            {
                effect.transform.position = collision.contacts[0].point;
                effect.transform.forward = collision.contacts[0].normal;
            }

            // 2) 데미지 처리
            var receiver = collision.collider.GetComponent<IDamageReceiver>();
            if (receiver != null)
            {
                IDamageData damageData = new DamageData(10f, gameObject); // 임시로 총알이 공격자
                receiver.ReceiveDamage(damageData);
            }

            Destroy(gameObject);
        }

        // private void OnCollisionEnter(Collision collision)
        // {
        //     // 자기(공격자 하위) 무시
        //     if (attacker && collision.transform.IsChildOf(attacker.transform))
        //         return;

        //     Debug.Log($"[Projectile] Hit {collision.collider.name} (layer={LayerMask.LayerToName(collision.collider.gameObject.layer)})");

        //     // --- 임팩트 FX (원래 로직 유지 / 보강) ---
        //     string material = null;
        //     var physMat = collision.collider ? collision.collider.sharedMaterial : null;
        //     if (physMat != null)
        //         material = materialNames.FirstOrDefault(type => physMat.name.Contains(type));

        //     if (EffectManager.Instance.GetEffect(material, out GameObject effect))
        //     {
        //         var contact = collision.contacts[0];
        //         effect.transform.position = contact.point + contact.normal * 0.01f;
        //         effect.transform.rotation = Quaternion.LookRotation(contact.normal);
        //         Destroy(effect, 2f);
        //     }

        //     // --- 데미지 전달 ---
        //     var receiver = collision.collider.GetComponentInParent<IDamageReceiver>();
        //     if (receiver != null)
        //     {
        //         var data = new DamageData(damage, attacker ? attacker : gameObject);
        //         receiver.ReceiveDamage(data);
        //     }

        //     Destroy(gameObject);
        // }

        private void OnDestroy()
        {
            Debug.Log($"[Projectile] Destroyed ({name}) at t={Time.time:F2}");
        }
    }
}


// namespace TEC
// {
//     public class Projectile : MonoBehaviour
//     {
//         public float speed = 100f;
//         public float lifeTime = 3f;
//         public static string[] materialNames = { "Wood", "Rock", "Dirt", "Metal" };

//         private float damage = 0f;
//         private GameObject attacker;
//         private Collider myCollider;

//         private void Awake()
//         {
//             myCollider = GetComponent<Collider>();
//         }

//         public void Initialize(float newDamage, GameObject newAttacker)
//         {
//             damage = newDamage;
//             attacker = newAttacker;

//             //내 총알에 맞는거 무시
//             if (myCollider && attacker)
//             {
//                 var cols = attacker.GetComponentsInChildren<Collider>();
//                 for (int i = 0; i < cols.Length; i++)
//                 {
//                     Physics.IgnoreCollision(myCollider, cols[i], true);
//                 }
//             }
//         }

//         private void Start()
//         {
//             Rigidbody rb = GetComponent<Rigidbody>();
//             rb.AddForce(transform.forward * speed, ForceMode.Impulse);

//             Destroy(gameObject, lifeTime);
//         }

//         private void OnCollisionEnter(Collision collision)
//         {

//             // Debug.Log(collision.gameObject.name);
//             //총알 충돌처리에 대한 구현
//             //TODO 1. Effect를 출력.
//             //TODO 2. Damage 처리하기 -> 캐릭터, 배경에 맞았는지
//             if (attacker && collision.transform.IsChildOf(attacker.transform))
//                 return;

//             // 1) 임팩트 이펙트(네 기존 로직 보강)
//             // string material = null;
//             // var physMat = collision.collider ? collision.collider.sharedMaterial : null;
//             // if (physMat != null)
//             //     material = materialNames.FirstOrDefault(type => physMat.name.Contains(type));

//             // if (EffectManager.Instance.GetEffect(material, out GameObject effect))
//             // {
//             //     var contact = collision.contacts[0];
//             //     effect.transform.position = contact.point + contact.normal * 0.01f;
//             //     effect.transform.rotation = Quaternion.LookRotation(contact.normal);
//             //     Destroy(effect, 2f);
//             // }

//             // 2) 데미지 전달
//             var receiver = collision.collider.GetComponentInParent<IDamageReceiver>();
//             if (receiver != null)
//             {
//                 var data = new DamageData(damage, attacker ? attacker : gameObject);
//                 receiver.ReceiveDamage(data);
//             }

//             string material = materialNames.FirstOrDefault(type => collision.collider.material.name.Contains(type));

//             if (EffectManager.Instance.GetEffect(material, out GameObject effect))
//             {
//                 effect.transform.position = collision.contacts[0].point;
//                 effect.transform.forward = collision.contacts[0].normal;
//             }
//             Destroy(gameObject);
//         }

//     }

// }
