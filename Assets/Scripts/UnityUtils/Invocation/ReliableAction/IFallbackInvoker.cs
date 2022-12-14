using System;
using System.Collections.Generic;

namespace UnityUtils.Invocation.ReliableAction
{
    public interface IFallbackInvoker
    {
        IReadOnlyDictionary<Guid, Type> SupportedActionTypes { get; }
    }
}