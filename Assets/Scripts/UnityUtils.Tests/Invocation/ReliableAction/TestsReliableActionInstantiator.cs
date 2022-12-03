using System;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    public class TestsReliableActionInstantiator : IReliableActionInstantiator
    {
        public IReliableAction Instantiate(Type type)
        {
            throw new NotImplementedException();
        }
    }
}