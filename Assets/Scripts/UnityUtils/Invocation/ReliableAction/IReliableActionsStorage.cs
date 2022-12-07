using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityUtils.Invocation.ReliableAction
{
    public interface IReliableActionsStorage
    {
        IReadOnlyList<IReliableAction> NewActions { get; }
        void Add([NotNull] IReliableAction action);
        bool Remove([NotNull] IReliableAction action);
        
        // List contains actions in backward order
        [MustUseReturnValue, CanBeNull]
        IList<IReliableAction> CreateAndTake([NotNull] IFallbackInvoker invoker, IReliableActionFallbackInstantiator instantiator);
    }
}