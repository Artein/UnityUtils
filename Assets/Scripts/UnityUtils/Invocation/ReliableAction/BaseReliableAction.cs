using System;

namespace UnityUtils.Invocation.ReliableAction
{
    public abstract class BaseReliableAction<T> : IReliableAction, IDeferredInvocation where T : struct
    {
        public IFallbackInvoker FallbackInvoker { get; }
        public bool IsCancelled { get; private set; }
        public bool IsInvoked { get; private set; }
        public bool IsLocked => _locksCount > 0;
        private readonly IReliableActionsStorage _storage;
        private readonly bool _isFallbackInvocation;
        private int _locksCount;

        public abstract Guid TypeGuid { get; }

        public bool TryInvoke()
        {
            if (CanBeInvoked())
            {
                if (!_isFallbackInvocation)
                {
                    _storage.Remove(this);
                }
                
                Invoke();
                IsInvoked = true;
                return true;
            }

            return false;
        }

        public IDisposable LockInvocation()
        {
            _locksCount += 1;
            // TODO Optimization: DisposableActions can use pools
            return new DisposableAction(UnlockInvocation);
        }

        public void Cancel()
        {
            IsCancelled = true;
            _storage.Remove(this);
        }

        public abstract void Save(string saveKey);
        public abstract void Load(string saveKey);
        public abstract void DeleteSave(string saveKey);
        
        protected BaseReliableAction(IReliableActionsStorage storage, IFallbackInvoker invoker, bool isFallbackInvocation = false, T? args = null)
        {
            _storage = storage;
            FallbackInvoker = invoker;
            _isFallbackInvocation = isFallbackInvocation;

            if (args.HasValue)
            {
                // ReSharper disable once VirtualMemberCallInConstructor
                OnConstructing(args.Value);
            }

            if (!_isFallbackInvocation)
            {
                _storage.Add(this);
            }
        }

        // This is actually a workaround to save data in children.
        // The problem is that we add this action in base constructor. Adding an action triggers save right away.
        // This leads to the situation where child data is not yet initialized but Save already received
        protected virtual void OnConstructing(T args)
        {
            // implement in child
        }

        protected virtual bool CanBeInvoked()
        {
            return !IsInvoked && !IsLocked && !IsCancelled;
        }

        protected abstract void Invoke();

        private void UnlockInvocation()
        {
            _locksCount -= 1;
            TryInvoke();
        }
    }
}