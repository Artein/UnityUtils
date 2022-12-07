using System;
using JetBrains.Annotations;

namespace UnityUtils.Invocation.ReliableAction
{
    // Separate interface for objects creation in case instantiation is not that simple.
    // For example, to instantiate an Action with custom arguments or via Zenject instantiation
    public interface IReliableActionFallbackInstantiator
    {
        [NotNull] IReliableAction Instantiate(Type type);
    }
}