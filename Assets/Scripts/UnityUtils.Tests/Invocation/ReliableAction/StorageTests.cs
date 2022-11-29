using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    public class StorageTests
    {
        private ReliableActionsStorage _storage;
        private IFallbackInvoker _invoker;

        [SetUp] public void Setup()
        {
            _storage = new ReliableActionsStorage();
            _invoker = Substitute.For<IFallbackInvoker>();
            PlayerPrefs.DeleteAll(); // TODO: Delete everything related to ReliableActionsStorage
        }
        
        [Test] public void ReliableActionsStorage_ContainsReliableAction_AfterItWasCreated()
        {
            var action = new TestsReliableAction(() => { }, _storage, _invoker);

            _storage.Actions.Should().ContainSingle(a => a.Equals(action));
        }

        [Test] public void ReliableActionsStorage_ContainsNoReliableAction_AfterItWasInvoked()
        {
            var action = new TestsReliableAction(() => { }, _storage, _invoker);
            action.TryInvoke();

            _storage.Actions.Should().BeEmpty();
        }
    }
}