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
            { TestsReliableAction.StaticTypeGuid, typeof(TestsReliableAction) },
        };

        // public to be invoked from tests
        public new void Invoke() => base.Invoke();
    }
}