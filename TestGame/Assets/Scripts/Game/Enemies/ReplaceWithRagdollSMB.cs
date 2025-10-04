using UnityEngine;

namespace Gamekit3D
{
    public class ReplaceWithRagdollSMB : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var replacer = animator.GetComponent<ReplaceWithRagdoll>();
            replacer.Replace();
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            var replacer = animator.GetComponent<ReplaceWithRagdoll>();
            replacer.Replace();
        }
    }
}