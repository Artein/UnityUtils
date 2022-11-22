using System;
using JetBrains.Annotations;

namespace UnityUtils.Invocation
{
    public class DisposableAction : IDisposable
    {
        private Action _action;

        public DisposableAction([NotNull] Action action)
        {
            _action = action;
        }
    
        public void Dispose()
        {
            if (_action != null)
            {
                _action.Invoke();
                _action = null;
            }
        }
    }
    
    public class DisposableAction<TArgs> : IDisposable 
        where TArgs : struct
    {
        private Action<TArgs> _action;
        private readonly TArgs _args;
            
        public DisposableAction([NotNull] Action<TArgs> action, TArgs args)
        {
            _action = action;
            _args = args;
        }
        
        public void Dispose()
        {
            if (_action != null)
            {
                _action.Invoke(_args);
                _action = null;
            }
        }
    }
}