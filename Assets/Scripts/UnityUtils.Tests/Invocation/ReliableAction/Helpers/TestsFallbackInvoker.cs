using System;
using System.Collections.Generic;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction.Helpers
{
    internal class TestsFallbackInvoker : BaseFallbackInvoker
    {
        public TestsFallbackInvoker(IReliableActionsStorage storage, IReliableActionFallbackInstantiator instantiator, bool logExceptionsDuringInvocation = true) 
            : base(storage, instantiator, logExceptionsDuringInvocation)
        {
        }

        protected override Dictionary<Guid, Type> SupportedActionTypesDic { get; } = new()
        {
            { TestsModel_IncrementCounter_ReliableAction.StaticTypeGuid, typeof(TestsModel_IncrementCounter_ReliableAction) },
            { ThrowsExceptionReliableAction.StaticTypeGuid, typeof(ThrowsExceptionReliableAction) },
        };

        // made public to be invoked from tests
        public new void Invoke() => base.Invoke();
    }
}