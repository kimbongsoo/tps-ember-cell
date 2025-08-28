using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace TEC
{
    public class Projectile : MonoBehaviour
    {
        public float speed = 100f;
        public float lifeTime = 3f;
        public static string[] materialNames = { "Wood", "Rock", "Dirt", "Metal" };
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
