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

            Transform targetPoint = brain.TargetCharacter.GetAvatarBoneTransform(HumanBodyBones.Spine);

            brain.AIController.SetAiming(targetPoint.position);
            brain.AIController.Fire();
        }
    }
}
