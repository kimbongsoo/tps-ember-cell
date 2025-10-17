using UnityEngine;

namespace TEC
{
    public class DeadStateMachineBehaviour : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            CharacterBase actor = animator.GetComponent<CharacterBase>();
            if (actor != null)
            {
                actor.Dead();
            }
        }
    }
}
