using System;
using UnityEngine;
using UnityUtils.Notification;

namespace UnityUtils.Examples.Notification
{
    public class DestroyNotifierExamples : MonoBehaviour, IDestroyNotifier<DestroyNotifierExamples>
    {
        public event Action<DestroyNotifierExamples> Destroying;

        private void OnDestroy()
        {
            Destroying?.Invoke(this);
        }
    }
}