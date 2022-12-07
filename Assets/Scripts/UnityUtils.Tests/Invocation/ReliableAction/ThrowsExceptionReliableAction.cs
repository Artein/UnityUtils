using System;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    public class ThrowsExceptionReliableAction : BaseReliableAction
    {
        public ThrowsExceptionReliableAction(IReliableActionsStorage storage, IFallbackInvoker invoker, bool isFallbackInvocation = false) : base(storage, invoker, isFallbackInvocation)
        {
        }

        public static readonly Guid StaticTypeGuid = new("FFF1B93C-ADDE-4561-BC57-9A18CEFA5788");
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
            throw new NotImplementedException();
        }
    }
}