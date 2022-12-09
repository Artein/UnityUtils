using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace UnityUtils.Invocation
{
    [DebuggerDisplay("_locksCount: {_locksCount} | _actionHandle: {_actionHandle}")]
    public class DeferredInvocation : IDeferredInvocation, IDisposable
    {
        private int _locksCount;
        private IDisposable _actionHandle;
#if UU_DI_STACKTRACE
        private readonly string _stackTraceOnCreation;
#endif 
        
        // Invocation is locked by default at creation. Call Dispose() to unlock
        public DeferredInvocation(Action action)
        {
            _locksCount = 1;
            _actionHandle = new DisposableAction(action);
#if UU_DI_STACKTRACE
            _stackTraceOnCreation = UnityEngine.StackTraceUtility.ExtractStackTrace();
#endif
        }

        // Invocation is locked by default at creation. Call Dispose() to unlock
        public DeferredInvocation([NotNull] IDisposableAction disposableAction)
        {
            _locksCount = 1;
            _actionHandle = disposableAction;
        }

        IDisposable IDeferredInvocation.LockInvocation()
        {
            _locksCount += 1;
            return new DisposableAction(UnlockInvocation);
        }

        public void Cancel()
        {
            _actionHandle = null;
        }

        public void Dispose()
        {
#if UU_DI_STACKTRACE
            UnityEngine.Debug.Log($"{_stackTraceOnCreation}");
#endif
            UnlockInvocation();
        }

        private void UnlockInvocation()
        {
            _locksCount -= 1;
            TryInvoke();
        }

        private void TryInvoke()
        {
            if (_locksCount == 0 && _actionHandle != null)
            {
                _actionHandle.Dispose();
                _actionHandle = null;
            }
        }
    }
}