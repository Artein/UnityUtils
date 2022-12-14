using System;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction.Helpers
{
    internal sealed class EmptyReliableAction : BaseReliableAction<EmptyArguments>
    {
        public EmptyReliableAction(IReliableActionsStorage storage, IFallbackInvoker invoker, bool isFallbackInvocation = false) 
            : base(storage, invoker, isFallbackInvocation)
        {
        }

        public static readonly Guid StaticTypeGuid = new("EFAA74C7-554C-41B6-8160-B87CC8E05F69");
        public override Guid TypeGuid => StaticTypeGuid;
        
        public override void Save(string saveKey)
        {
            // nothing to save
        }

        public override void Load(string saveKey)
        {
            // nothing to load
        }

        public override void DeleteSave(string saveKey)
        {
            // nothing to delete
        }

        protected override void Invoke()
        {
            // nothing to do
        }
    }
}