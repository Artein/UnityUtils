using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityUtils.Invocation.ReliableAction
{
    public interface IReliableActionsStorage
    {
        void Add([NotNull] IReliableAction action);
        void Remove([NotNull] IReliableAction action);
        IList<IReliableAction> Take([NotNull] IFallbackInvoker invoker);
    }
}