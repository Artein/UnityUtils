using System;
using System.Text;
using JetBrains.Annotations;

namespace UnityUtils.Invocation
{
    public interface IDisposableAction : IDisposable
    {
        bool IsDisposed { get; }
    }
    
    public class DisposableAction : IDisposableAction
    {
        private Action _action;
        public bool IsDisposed => _action == null;

        public DisposableAction([NotNull] Action action)
        {
            _action = action;
        }
    
        void IDisposable.Dispose()
        {
            if (!IsDisposed)
            {
                _action.Invoke();
                _action = null;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"{nameof(IsDisposed)}: ");
            builder.Append(IsDisposed);
            builder.Append($"; {nameof(_action)}: ");
            builder.Append(IsDisposed ? "null" : $"{_action.Target}___{_action.Method}");
            return builder.ToString();
        }
    }
    
    public class DisposableAction<TArgs> : IDisposableAction 
        where TArgs : struct
    {
        private Action<TArgs> _action;
        private readonly TArgs _args;
        public bool IsDisposed => _action == null;
            
        public DisposableAction([NotNull] Action<TArgs> action, TArgs args)
        {
            _action = action;
            _args = args;
        }
        
        void IDisposable.Dispose()
        {
            if (!IsDisposed)
            {
                _action.Invoke(_args);
                _action = null;
            }
        }
    }
}