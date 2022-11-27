using UnityEngine;
using UnityEngine.TestTools;

namespace UnityUtils.Notification.Animator
{
    [ExcludeFromCoverage] // no need to test Unity itself
    public class EnteredStateAnimatorNotifier : StateMachineBehaviour
    {
        public event EnteredEventHandler Entered;

        // UnityDoc: Called on the first Update frame when a state machine evaluate this state
        public override void OnStateExit(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Entered?.Invoke(animator, stateInfo, layerIndex);
        }

        public delegate void EnteredEventHandler(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
}