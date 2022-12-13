using System;
using System.Collections.Generic;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction.Helpers
{
    internal class SecondTestsFallbackInvoker : BaseFallbackInvoker
    {
        public SecondTestsFallbackInvoker(IReliableActionsStorage storage, IReliableActionFallbackInstantiator instantiator, bool logExceptionsDuringInvocation = true) 
            : base(storage, instantiator, logExceptionsDuringInvocation)
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