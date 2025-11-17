using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class RollingStateMachineBehaviour : StateMachineBehaviour
    {
        private CharacterBase linkedCharacter;
        public void Initialize(CharacterBase character)
        {
            linkedCharacter = character;

        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (linkedCharacter == null)
                linkedCharacter = animator.GetComponentInParent<CharacterBase>();
            linkedCharacter.RollingComplete();
        }
    }
}
