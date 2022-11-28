using System;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    internal class TestsReliableAction : BaseReliableAction
    {
        public TestsReliableAction(Action action, IReliableActionsStorage storage, IFallbackInvoker invoker, bool isFallbackInvocation = false) 
            : base(storage, invoker, isFallbackInvocation)
        {
            _action = action;
        }

        private readonly Action _action;
        public override Guid TypeGuid => new("C3B7E643-9358-4FCF-9337-9BA6403F1F11");

        public override void Save(string saveKey)
        {
            // nothing to save
        }

        public override void Load(string saveKey) => throw new NotImplementedException();
        public override void DeleteSave(string saveKey) => throw new NotImplementedException();

        protected override void Invoke()
        {
            _action.Invoke();
        }
    }
}