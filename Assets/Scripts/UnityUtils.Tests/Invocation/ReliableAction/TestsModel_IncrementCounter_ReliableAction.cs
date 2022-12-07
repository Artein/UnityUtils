using System;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    internal class TestsModel_IncrementCounter_ReliableAction : BaseReliableAction
    {
        public TestsModel_IncrementCounter_ReliableAction(TestsModel testsModel, IReliableActionsStorage storage, IFallbackInvoker invoker, bool isFallbackInvocation = false) 
            : base(storage, invoker, isFallbackInvocation)
        {
            _testsModel = testsModel;
        }

        public static readonly Guid StaticTypeGuid = new("C3B7E643-9358-4FCF-9337-9BA6403F1F11");
        private readonly TestsModel _testsModel;
        public override Guid TypeGuid => StaticTypeGuid;

        public override void Save(string saveKey)
        {
            // nothing to save
        }

        public override void Load(string saveKey)
        {
            // nothing to do
        }

        public override void DeleteSave(string saveKey)
        {
            // nothing to do
        }

        protected override void Invoke()
        {
            _testsModel.Count += 1;
        }
    }
}