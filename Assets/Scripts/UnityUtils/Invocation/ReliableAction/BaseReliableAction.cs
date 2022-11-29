using System;

namespace UnityUtils.Invocation.ReliableAction
{
    public abstract class BaseReliableAction : IReliableAction, IDeferredInvocation
    {
        public IFallbackInvoker FallbackInvoker { get; }
        public bool IsCancelled { get; private set; }
        public bool IsInvoked { get; private set; }
        public bool IsLocked => _locksCount > 0;
        private readonly IReliableActionsStorage _storage;
        private readonly bool _isFallbackInvocation;
        private int _locksCount;

        public abstract Guid TypeGuid { get; }

        protected BaseReliableAction(IReliableActionsStorage storage, IFallbackInvoker invoker, bool isFallbackInvocation = false)
        {
            _isFallbackInvocation = isFallbackInvocation;
            FallbackInvoker = invoker;
            _storage = storage;

            if (!_isFallbackInvocation)
            {
                storage.Add(this);
            }
        }

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