using System;
using System.Collections.Generic;

namespace UnityUtils.Invocation.ReliableAction
{
    public abstract class BaseFallbackInvoker : IFallbackInvoker
    {
        private readonly IReliableActionsStorage _storage;
        
        public bool IsTypeGuidSupported(Guid guid) => SupportedActionTypes.ContainsKey(guid);

        protected abstract Dictionary<Guid, Type> SupportedActionTypesDic { get; }
        public IReadOnlyDictionary<Guid, Type> SupportedActionTypes => SupportedActionTypesDic;

        protected BaseFallbackInvoker(IReliableActionsStorage storage)
        {
            _storage = storage;
        }
        
        protected void Invoke()
        {
            var actions = _storage.Take(this);
            if (actions == null)
            {
                return;
            }
            
            for (int i = 0; i < actions.Count; i += 1)
            {
                var action = actions[i];
                action.TryInvoke();
            }
        }
    }
}