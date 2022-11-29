using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using UnityUtils.Invocation.ReliableAction;
using Is = NUnit.Framework.Is;

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

        [Test] public void ReliableActionsStorage_ContainsNoReliableAction_AfterItWasCancelled()
        {
            var action = new TestsReliableAction(() => { }, _storage, _invoker);
            action.Cancel();

            _storage.Actions.Should().BeEmpty();
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

            _storage.Actions.Should().NotBeEmpty()
                .And.HaveCount(1)
                .And.Contain(action2)
                .And.NotContain(action)
                .And.NotContain(action3);
        }

        [Test] public void ReliableActionsStorage_TakeMethod_DoesNotAllocateGC()
        {
            var invoker2 = Substitute.For<IFallbackInvoker>();
            var _ = new TestsReliableAction(() => { }, _storage, _invoker);
            var __ = new TestsReliableAction(() => { }, _storage, invoker2);
            var ___ = new TestsReliableAction(() => { }, _storage, _invoker);

            Assert.That(() =>
            {
                var ____ = _storage.Take(_invoker);
            }, Is.Not.AllocatingGCMemory());
        }

        [Test] public void ReliableActionStorage_OnCreation_LoadsAllSavedActions()
        {
            throw new NotImplementedException();
        }
    }
}