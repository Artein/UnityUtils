using System;

namespace UnityUtils.Invocation.ReliableAction
{
    public interface IReliableAction : ISavable
    {
        Guid TypeGuid { get; } // TODO C#8: can be static
        IFallbackInvoker FallbackInvoker { get; }
        bool TryInvoke();
        void Cancel();
    }
}