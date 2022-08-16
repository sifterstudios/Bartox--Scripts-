using UnityEngine;

namespace Bartox.Animation.State
{
    public class ResetAnimation : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.Play("Locomotion", 0);
            // animator.WriteDefaultValues();
        }
    }
}