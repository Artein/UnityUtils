using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    [TestFixture]
    public class StorageTests
    {
        private ReliableActionsStorage _storage;
        private IFallbackInvoker _invoker;
        private readonly TestsReliableActionInstantiator _instantiator = new(); // has no caches

        [SetUp] public void OnSetUp()
        {
            _storage = new ReliableActionsStorage(_instantiator);
            _invoker = Substitute.For<IFallbackInvoker>();
        }

        [TearDown] public void OnTearDown()
        {
            PlayerPrefs.DeleteAll(); // TODO: Delete everything related ONLY to ReliableActionsStorage
        }
        
        [Test] public void ReliableActionsStorage_ContainsReliableAction_AfterItWasCreated()
        {
            var action = new TestsReliableAction(() => { }, _storage, _invoker);

            _storage.NewActions.Should().ContainSingle(a => a.Equals(action));
        }

        [Test] public void ReliableActionsStorage_ContainsNoReliableAction_AfterItWasInvoked()
        {
            var action = new TestsReliableAction(() => { }, _storage, _invoker);
            action.TryInvoke();

            _storage.NewActions.Should().BeEmpty();
        }

        [Test] public void ReliableActionsStorage_ContainsNoReliableAction_AfterItWasCancelled()
        {
            var action = new TestsReliableAction(() => { }, _storage, _invoker);
            action.Cancel();

            _storage.NewActions.Should().BeEmpty();
        }

        [Test] public void ReliableActionsStorage_TakeMethod_ReturnsAllActions_WithPassedFallbackInvoker()
        {
            var invoker2 = Substitute.For<IFallbackInvoker>();
            var action = new TestsReliableAction(() => { }, _storage, _invoker);
            var action2 = new TestsReliableAction(() => { }, _storage, invoker2);
            var action3 = new TestsReliableAction(() => { }, _storage, _invoker);

            var takenActions = _storage.Take(_invoker);

            takenActions.Should().NotBeEmpty()
                .And.HaveCount(2)
                .And.Contain(action)
                .And.Contain(action3)
                .And.NotContain(action2);
        }

        [Test] public void ReliableActionsStorage_HasNoActions_After_TakeMethod_WithPassedFallbackInvoker()
        {
            var invoker2 = Substitute.For<IFallbackInvoker>();
            var action = new TestsReliableAction(() => { }, _storage, _invoker);
            var action2 = new TestsReliableAction(() => { }, _storage, invoker2);
            var action3 = new TestsReliableAction(() => { }, _storage, _invoker);

            var _ = _storage.Take(_invoker);

            _storage.NewActions.Should().NotBeEmpty()
                .And.HaveCount(1)
                .And.Contain(action2)
                .And.NotContain(action)
                .And.NotContain(action3);
        }

        [Test] public void ReliableActionStorage_OnCreation_LoadsAllSavedActions()
        {
            throw new NotImplementedException();
        }
        
        // TODO: Add saving and loading tests
        // TODO: Add fallback invocation tests
    }
}