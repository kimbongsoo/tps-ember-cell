using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class UnarmedStateMachineBehaviour : StateMachineBehaviour
    {
        private CharacterBase linkedCharacter;
        public void Initialize(CharacterBase character)
        {
            linkedCharacter = character;

        }
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetLayerWeight(2, 0f);
        }
    }
}
