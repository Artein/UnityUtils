using FluentAssertions;
using Invocation.ReliableAction.Helpers;
using NSubstitute;
using NUnit.Framework;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    [TestFixture] public class StorageTests
    {
        private IReliableActionsStorage _storage;
        private IFallbackInvoker _invoker;
        private TestsModel _testsModel;
        private IReliableActionFallbackInstantiator _fallbackInstantiator;

        [SetUp] public void OnSetUp()
        {
            _storage = new ReliableActionsStorage();
            _invoker = Substitute.For<IFallbackInvoker>();
            _testsModel = new TestsModel();
            _fallbackInstantiator = new TestsReliableActionFallbackInstantiator(_testsModel, _storage);
        }

        [TearDown] public void OnTearDown()
        {
            _storage.Clear();
        }
        
        [Test] public void ReliableActionsStorage_ContainsReliableAction_AfterItWasCreated()
        {
            var reliableAction = new TestsModel_IncrementCounter_ReliableAction(_testsModel, _storage, _invoker);

            _storage.NewActions.Should().ContainSingle(a => a.Equals(reliableAction));
        }

        [Test] public void ReliableActionsStorage_ContainsNoReliableAction_AfterItWasInvoked()
        {
            var action = new TestsModel_IncrementCounter_ReliableAction(_testsModel, _storage, _invoker);
            action.TryInvoke();

            _storage.NewActions.Should().BeEmpty();
        }

        [Test] public void ReliableActionsStorage_ContainsNoReliableAction_AfterItWasCancelled()
        {
            var action = new TestsModel_IncrementCounter_ReliableAction(_testsModel, _storage, _invoker);
            action.Cancel();

            _storage.NewActions.Should().BeEmpty();
        }

        [Test] public void ReliableActionsStorage_TakeMethod_ReturnsAllActions_WithPassedFallbackInvoker()
        {
            var invoker2 = Substitute.For<IFallbackInvoker>();
            var action = new TestsModel_IncrementCounter_ReliableAction(_testsModel, _storage, _invoker);
            var action2 = new TestsModel_IncrementCounter_ReliableAction(_testsModel, _storage, invoker2);
            var action3 = new TestsModel_IncrementCounter_ReliableAction(_testsModel, _storage, _invoker);

            var takenActions = _storage.CreateAndTake(_invoker, _fallbackInstantiator);

            takenActions.Should().NotBeEmpty()
                .And.HaveCount(2)
                .And.Contain(action)
                .And.Contain(action3)
                .And.NotContain(action2);
        }

        [Test] public void ReliableActionsStorage_HasNoActions_After_TakeMethod_WithPassedFallbackInvoker()
        {
            var invoker2 = Substitute.For<IFallbackInvoker>();
            var action = new TestsModel_IncrementCounter_ReliableAction(_testsModel, _storage, _invoker);
            var action2 = new TestsModel_IncrementCounter_ReliableAction(_testsModel, _storage, invoker2);
            var action3 = new TestsModel_IncrementCounter_ReliableAction(_testsModel, _storage, _invoker);

            var _ = _storage.CreateAndTake(_invoker, _fallbackInstantiator);

            _storage.NewActions.Should().NotBeEmpty()
                .And.HaveCount(1)
                .And.Contain(action2)
                .And.NotContain(action)
                .And.NotContain(action3);
        }

        [Test] public void ReliableActionsStorage_Remove_Method_ReturnsFalse_WhenNothingWasRemoved()
        {
            var secondStorage = new ReliableActionsStorage();
            var actionFromSecondStorage = new TestsModel_IncrementCounter_ReliableAction(_testsModel, secondStorage, _invoker);
            
            var wasRemoved = _storage.Remove(actionFromSecondStorage);

            wasRemoved.Should().Be(false);
        }
    }
}