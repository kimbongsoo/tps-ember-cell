using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TEC
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 100f;
        public float lifeTime = 3f;
        public static string[] materialNames = { "Wood", "Rock", "Dirt", "Metal" };

        private float damage = 0f;
        private GameObject attacker;
 
        public void Initialize(GameObject attacker, float damage)
        {
            this.attacker = attacker;
            this.damage = damage;
        }

        private void Start()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * speed, ForceMode.Impulse);

            Destroy(gameObject, lifeTime);
        }

        private void OnCollisionEnter(Collision collision)
        {

            // Debug.Log(collision.gameObject.name);
            //총알 충돌처리에 대한 구현
            //TODO 1. Effect를 출력.
            //TODO 2. Damage 처리하기 -> 캐릭터, 배경에 맞았는지
            if (attacker && collision.transform.IsChildOf(attacker.transform))
                return;

            // 임팩트 이펙트 -> 이펙트매니저로 책임전가
            // string material = null;
            // var physMat = collision.collider ? collision.collider.sharedMaterial : null;
            // if (physMat != null)
            //     material = materialNames.FirstOrDefault(type => physMat.name.Contains(type));

            // if (EffectManager.Instance.GetEffect(material, out GameObject effect))
            // {
            //     var contact = collision.contacts[0];
            //     effect.transform.position = contact.point + contact.normal * 0.01f;
            //     effect.transform.rotation = Quaternion.LookRotation(contact.normal);
            //     Destroy(effect, 2f);
            // }

            //데미지 전달
            // collision.transform.root.GetComponent<IDamageReceiver>();
            var receiver = collision.collider.GetComponentInParent<IDamageReceiver>();
            if (receiver != null)
            {
                var data = new DamageData(damage, attacker);
                receiver.ReceiveDamage(data);
            }

            string material = materialNames.FirstOrDefault(type => collision.collider.material.name.Contains(type));

            if (EffectManager.Instance.GetEffect(material, out GameObject effect))
            {
                effect.transform.position = collision.contacts[0].point;
                effect.transform.forward = collision.contacts[0].normal;
            }
            Destroy(gameObject);
        }

    }

}
