using System;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    public class TestsReliableActionFallbackInstantiator : IReliableActionFallbackInstantiator
    {
        private readonly TestsModel _testsModel;
        private readonly IReliableActionsStorage _storage;

        public TestsReliableActionFallbackInstantiator(TestsModel testsModel, IReliableActionsStorage storage)
        {
            _storage = storage;
            _testsModel = testsModel;
        }
        
        public IReliableAction Instantiate(Type type)
        {
            if (type == typeof(TestsModel_IncrementCounter_ReliableAction))
            {
                var reliableAction = new TestsModel_IncrementCounter_ReliableAction(_testsModel, _storage, null, true);
                return reliableAction;
            }

            if (type == typeof(ThrowsExceptionReliableAction))
            {
                var reliableAction = new ThrowsExceptionReliableAction(_storage, null, true);
                return reliableAction;
            }

            throw new NotImplementedException();
        }
    }
}