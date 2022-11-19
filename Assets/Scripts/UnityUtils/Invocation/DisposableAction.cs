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
    
    public class DisposableAction<T> : IDisposable where T : Delegate
    {
        private T _action;
        private readonly object[] _args;
            
        public DisposableAction([NotNull] T action, params object[] args)
        {
            _action = action;

            if (args.Length > 0)
            {
                _args = new object[args.Length];
                args.CopyTo(_args, 0);
            }
            else
            {
                _args = Array.Empty<object>();
            }
        }
        
        public void Dispose()
        {
            if (_action != null)
            {
                _action.DynamicInvoke(_args);
                _action = null;
            }
        }
    }
}