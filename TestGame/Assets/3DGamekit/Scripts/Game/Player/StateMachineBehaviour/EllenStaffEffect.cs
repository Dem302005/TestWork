using UnityEngine;

namespace Gamekit3D
{
    public class EllenStaffEffect : StateMachineBehaviour
    {
        public int effectIndex;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var ctrl = animator.GetComponent<PlayerController>();

            ctrl.meleeWeapon.effects[effectIndex].Activate();
        }
    }
}