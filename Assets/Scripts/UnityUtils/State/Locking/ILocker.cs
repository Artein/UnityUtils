using System;
using JetBrains.Annotations;

namespace UnityUtils.State.Locking
{
    public interface ILocker
    {
        bool IsLocked { get; }
        
        [MustUseReturnValue("Release disposable handle to Unlock")]
        IDisposable Lock();
    }
}