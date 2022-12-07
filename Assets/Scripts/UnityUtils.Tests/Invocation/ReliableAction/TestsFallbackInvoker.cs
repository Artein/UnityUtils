using System;
using System.Collections.Generic;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    internal class TestsFallbackInvoker : BaseFallbackInvoker
    {
        public TestsFallbackInvoker(IReliableActionsStorage storage) : base(storage)
        {
        }

        protected override Dictionary<Guid, Type> SupportedActionTypesDic { get; } = new()
        {
            { TestsModel_IncrementCounter_ReliableAction.StaticTypeGuid, typeof(TestsModel_IncrementCounter_ReliableAction) },
        };

        // made public to be invoked from tests
        public new void Invoke() => base.Invoke();
    }
}