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
        private readonly bool _actionInvocationOnce;
        public bool IsInvoked { get; private set; }

        private bool CanBeInvoked => !_actionInvocationOnce || !IsInvoked;

        public DisposableAction([NotNull] Action action, bool actionInvocationOnce = true)
        {
            _action = action;
            _actionInvocationOnce = actionInvocationOnce;
        }

        public void Reset()
        {
            IsInvoked = false;
        }
    
        public void Dispose()
        {
            if (CanBeInvoked)
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
        private readonly bool _actionInvocationOnce;
        
        private bool CanBeInvoked => !_actionInvocationOnce || !IsInvoked;
        
        public bool IsInvoked { get; private set; }
            
        public DisposableAction([NotNull] Action<TArgs> action, TArgs args, bool actionInvocationOnce = true)
        {
            _action = action;
            _args = args;
            _actionInvocationOnce = actionInvocationOnce;
        }

        public void Reset()
        {
            IsInvoked = false;
        }
        
        public void Dispose()
        {
            if (CanBeInvoked)
            {
                IsInvoked = true;
                _action.Invoke(_args);
            }
        }
    }
}