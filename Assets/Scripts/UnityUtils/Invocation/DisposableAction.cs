using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace UnityUtils.Invocation
{
    public interface IDisposableAction : IDisposable
    {
        bool IsInvoked { get; }
    }
    
    [DebuggerDisplay("IsDisposed: {IsInvoked} | " +
                     "_action: {_action == null ? null : _action.Target+\"___\"+_action.Method}")]
    public class DisposableAction : IDisposableAction
    {
        private readonly Action _action;
        private readonly Action _resetAction;
        public bool IsInvoked { get; private set; }

        public DisposableAction([NotNull] Action action)
        {
            _action = action;
        }
    
        public void Dispose()
        {
            if (!IsInvoked)
            {
                IsInvoked = true;
                _action.Invoke();
            }
        }
    }
    
    [DebuggerDisplay("IsDisposed: {IsInvoked} | " +
                     "_action: {_action == null ? null : _action.Target+\"___\"+_action.Method} | " +
                     "_args: {_args}")]
    public class DisposableAction<TArgs> : IDisposableAction 
        where TArgs : struct
    {
        private readonly Action<TArgs> _action;
        private readonly TArgs _args;
        public bool IsInvoked { get; private set; }
            
        public DisposableAction([NotNull] Action<TArgs> action, TArgs args)
        {
            _action = action;
            _args = args;
        }
        
        public void Dispose()
        {
            if (!IsInvoked)
            {
                IsInvoked = true;
                _action.Invoke(_args);
            }
        }
    }
}