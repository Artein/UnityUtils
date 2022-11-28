using System;
using JetBrains.Annotations;

namespace UnityUtils.Invocation
{
    public interface IDeferredInvocation
    {
        [MustUseReturnValue("Dispose handle to continue invocation")]
        IDisposable LockInvocation();
    }
}