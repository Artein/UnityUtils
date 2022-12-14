using System;
using System.Collections.Generic;
using Assert = UnityEngine.Assertions.Assert;

namespace UnityUtils.Invocation.ReliableAction
{
    public abstract class BaseFallbackInvoker : IFallbackInvoker
    {
        private readonly IReliableActionsStorage _storage;
        private readonly IReliableActionFallbackInstantiator _instantiator;

        protected abstract Dictionary<Guid, Type> SupportedActionTypesDic { get; }
        public IReadOnlyDictionary<Guid, Type> SupportedActionTypes => SupportedActionTypesDic;

        protected BaseFallbackInvoker(IReliableActionsStorage storage, IReliableActionFallbackInstantiator instantiator)
        {
            _instantiator = instantiator;
            _storage = storage;
        }
        
        protected void Invoke()
        {
            var actions = _storage.CreateAndTake(this, _instantiator);
            Assert.IsNotNull(actions);
            
            for (int i = 0; i < actions.Count; i += 1)
            {
                // For now, I don't want to break iteration over all actions even though one of them is failed (throws exception)
                try
                {
                    var action = actions[i];
                    action.TryInvoke();
                }
                catch (Exception exception) // hide and just log exception
                {
                    UnityEngine.Debug.LogException(exception);
                }
            }
        }
    }
}