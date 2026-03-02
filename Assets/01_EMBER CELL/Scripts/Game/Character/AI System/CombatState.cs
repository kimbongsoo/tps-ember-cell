using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class CombatState : IState
    {
        public CombatState(AIBrain brain) => this.brain = brain;

        public AIBrain AIBrain => brain;
        private AIBrain brain;

        public void Enter()
        {
            //TODO : NavAgent 움직임을 멈춤
            brain.AIController.Stop();

            //TODO : Target 캐릭터를 공격
            brain.AIController.EquipWeapon();
        }

        public void Exit()
        {
            brain.AIController.UnEquipWeapon();
        }

        public void Update()
        {
            if (brain.TargetCharacter == null)
                return;

            //TODO : Target 캐릭터를 쳐다봄
            Vector3 direction = brain.TargetCharacter.transform.position - brain.transform.position;
            direction.y = 0; //y축 회전 방지
            brain.transform.forward = direction;

            // Transform targetPoint = brain.TargetCharacter.GetAvatarBoneTransform(HumanBodyBones.Spine);
            Transform targetPoint = GetRandomAimBoneTransform(brain.TargetCharacter);
            if (targetPoint == null)
                return;

            brain.AIController.SetAiming(targetPoint.position);
            brain.AIController.Fire();
        }

        private Transform GetRandomAimBoneTransform(CharacterBase targetCharacter)
        {
            float weight = Random.Range(0f, 1f);

            if (weight <= 1f / 6f)
            {
                return targetCharacter.GetAvatarBoneTransform(HumanBodyBones.Head);
            }

            if (weight <= 2f / 6f)
            {
                return targetCharacter.GetAvatarBoneTransform(HumanBodyBones.Chest);
            }

            if (weight <= 3f / 6f)
            {
                return targetCharacter.GetAvatarBoneTransform(HumanBodyBones.RightUpperArm);
            }

            if (weight <= 4f / 6f)
            {
                return targetCharacter.GetAvatarBoneTransform(HumanBodyBones.LeftUpperArm);
            }

            if (weight <= 5f / 6f)
            {
                return targetCharacter.GetAvatarBoneTransform(HumanBodyBones.RightUpperLeg);
            }
            return targetCharacter.GetAvatarBoneTransform(HumanBodyBones.LeftUpperLeg);
        }
    }
}
