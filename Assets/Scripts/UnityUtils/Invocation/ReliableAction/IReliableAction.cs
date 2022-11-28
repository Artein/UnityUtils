using System;

namespace UnityUtils.Invocation.ReliableAction
{
    public interface IReliableAction : ISavable
    {
        Guid TypeGuid { get; } // TODO C#8: can be static
        bool TryInvoke();
        void Cancel();
    }
}