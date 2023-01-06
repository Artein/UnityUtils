using System;
using UnityEngine.Assertions;
using UnityUtils.Invocation;

namespace UnityUtils.State.Locking
{
    public class Locker : ILocker, IDisposable
    {
        private readonly bool _lockedByDefault;
        private DisposableAction<int> _unlockDisposableAction;
        private int _reincarnation;
        private bool _isDisposed;
        private int _locksCount;

        public bool IsLocked => _locksCount > 0;
        
        public Locker(bool lockedByDefault = true)
        {
            _lockedByDefault = lockedByDefault;
            
            _locksCount = _lockedByDefault ? 1 : 0;
            _unlockDisposableAction = new DisposableAction<int>(Unlock, _reincarnation);
        }
    
        public IDisposable Lock()
        {
            _locksCount += 1;
            return _unlockDisposableAction;
        }

        public void Reset()
        {
            _isDisposed = false;
            _locksCount = _lockedByDefault ? 1 : 0;
            _reincarnation += 1;
            _unlockDisposableAction = new DisposableAction<int>(Unlock, _reincarnation);
        }
    
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                if (_lockedByDefault)
                {
                    Unlock(_reincarnation);
                }
            }
        }
    
        private void Unlock(int reincarnation)
        {
            if (reincarnation != _reincarnation)
            {
                // Unlock from previous reincarnation. Skip it
                // This is rare case when somebody Locked this and has not freed DisposableHandle, but Locker was Reset after that
                return;
            }
            
            Assert.IsTrue(_locksCount >= 1);
            _locksCount -= 1;
        }
    }
}