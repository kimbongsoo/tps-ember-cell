using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace TEC
{
    public class NpcCharacter : MonoBehaviour, IDamageReceiver
    {
        [SerializeField] private float maxHP = 50f;
        private float currentHP;

        [Header("Components")]
        private Animator characterAnimator;
        private CharacterController characterController;
        private RigBuilder rigBuilder;

        private bool isDead = false;

        private void Awake()
        {
            if (!characterAnimator) characterAnimator = GetComponent<Animator>();
            if (!characterController) characterController = GetComponent<CharacterController>();
            if (!rigBuilder) rigBuilder = GetComponent<RigBuilder>();
        }

        private void Start()
        {
            currentHP = maxHP;
        }

        public void ReceiveDamage(IDamageData data)
        {
            if (isDead) return;

            currentHP -= data.DamageAmount;
            if (currentHP <= 0)
            {
                Dead();
            }
        }

        private void Dead()
        {
            if (isDead) return;
            isDead = true;

            // NPC 제어 시스템 OFF
            if (characterController != null)
                characterController.enabled = false;

            if (rigBuilder != null)
                rigBuilder.enabled = false;

            if (characterAnimator != null)
                characterAnimator.enabled = false;

            // Ragdoll이 Animator OFF로 자동 활성화되는 구조일 때
            Debug.Log("NPC Dead → Animator OFF → Ragdoll 작동");
        }
    }
}
