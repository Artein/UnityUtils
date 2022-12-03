using System;

namespace UnityUtils.Invocation.ReliableAction
{
    public interface IReliableActionsSaveMapper
    {
        Type FindType(Guid typeGuid);
    }
}