using UnityEngine;
using UnityEngine.TestTools;

namespace UnityUtils.Notification.Animator
{
    [ExcludeFromCoverage] // no need to test Unity itself
    public class ExitedStateAnimatorNotifier : StateMachineBehaviour
    {
        public event ExitedEventHandler Exited;

        // UnityDoc: Called on the last update frame when a state machine evaluate this state
        public override void OnStateExit(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Exited?.Invoke(animator, stateInfo, layerIndex);
        }

        public delegate void ExitedEventHandler(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
}