using System;
using System.Collections.Generic;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction.Helpers
{
    internal class SecondTestsFallbackInvoker : BaseFallbackInvoker
    {
        public SecondTestsFallbackInvoker(IReliableActionsStorage storage, IReliableActionFallbackInstantiator instantiator) 
            : base(storage, instantiator)
        {
        }

        protected override Dictionary<Guid, Type> SupportedActionTypesDic { get; } = new()
        {
            { EmptyReliableAction.StaticTypeGuid, typeof(EmptyReliableAction) },
        };

        // made public to be invoked from tests
        public new void Invoke() => base.Invoke();
    }
}