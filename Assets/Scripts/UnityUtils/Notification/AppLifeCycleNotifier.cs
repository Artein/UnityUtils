using System;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityUtils.Notification
{
    [ExcludeFromCoverage] // no need to test Unity itself
    public class AppLifeCycleNotifier : MonoBehaviour, IAppLifeCycleNotifier
    {
        [SerializeField] private bool _enableLogging;

        public event IAppLifeCycleNotifier.AppFocusChangedEventHandler ApplicationFocusChanged;
        public event IAppLifeCycleNotifier.AppPauseStateChangedEventHandler ApplicationPauseStateChanged;
        public event Action ApplicationQuitting;

        public bool HasFocus => Application.isFocused;
        public bool IsPaused { get; private set; }
        public bool IsQuitting { get; private set; }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (_enableLogging)
            {
                Debug.Log($"{nameof(AppLifeCycleNotifier)}: Focus changed | {nameof(hasFocus)}:{hasFocus}");
            }

            ApplicationFocusChanged?.Invoke(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            IsPaused = pauseStatus;

            if (_enableLogging)
            {
                Debug.Log($"{nameof(AppLifeCycleNotifier)}: Pause state changed | {nameof(pauseStatus)}:{pauseStatus}");
            }

            ApplicationPauseStateChanged?.Invoke(pauseStatus);
        }

        private void OnApplicationQuit()
        {
            IsQuitting = true;

            if (_enableLogging)
            {
                Debug.Log($"{nameof(AppLifeCycleNotifier)}: Received quit");
            }

            ApplicationQuitting?.Invoke();
        }
    }
}