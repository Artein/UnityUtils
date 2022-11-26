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
        
        // Invocation is locked by default at creation. Call Dispose() to unlock
        public DeferredInvocation(Action action)
        {
            _locksCount = 1;
            _actionHandle = new DisposableAction(action);
        }

        // Invocation is locked by default at creation. Call Dispose() to unlock
        public DeferredInvocation([NotNull] IDisposableAction disposableAction)
        {
            _locksCount = 1;
            _actionHandle = disposableAction;
        }

        IDisposable IDeferredInvocation.Lock()
        {
            _locksCount += 1;
            // TODO Optimization: DisposableActions can use pools
            return new DisposableAction(Unlock);
        }

        public void Cancel()
        {
            _actionHandle = null;
        }

        public void Dispose()
        {
            Unlock();
        }

        private void Unlock()
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