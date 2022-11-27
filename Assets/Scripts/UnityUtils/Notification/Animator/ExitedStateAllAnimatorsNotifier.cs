using UnityEngine;
using UnityEngine.TestTools;

namespace UnityUtils.Notification.Animator
{
    [SharedBetweenAnimators] // instantiated only once and shared among all Animator instances
    [ExcludeFromCoverage] // no need to test Unity itself
    public class ExitedStateAllAnimatorsNotifier : StateMachineBehaviour
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