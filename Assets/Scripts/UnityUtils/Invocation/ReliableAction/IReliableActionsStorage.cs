using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityUtils.Invocation.ReliableAction
{
    public interface IReliableActionsStorage
    {
        void Add([NotNull] IReliableAction action);
        bool Remove([NotNull] IReliableAction action);
        [MustUseReturnValue, CanBeNull]
        IList<IReliableAction> Take([NotNull] IFallbackInvoker invoker);
    }
}