using UnityEngine;

namespace Gamekit3D
{
    public class EllenSetTargetableSMB : StateMachineBehaviour
    {
        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var controller = animator.GetComponent<PlayerController>();

            if (controller != null) controller.RespawnFinished();
        }
    }
}