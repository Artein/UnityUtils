using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    public class InvocationTests
    {
        private int _methodCallsCount;

        [SetUp] public void Setup()
        {
            _methodCallsCount = 0;
        }

        [Test] public void ReliableAction_Invokes()
        {
            var storage = Substitute.For<IReliableActionsStorage>();
            var fallbackInvoker = Substitute.For<IFallbackInvoker>();
            var reliableAction = new TestReliableAction(IncrementCallsCount, storage, fallbackInvoker);

            reliableAction.TryInvoke();

            _methodCallsCount.Should().Be(1);
        }

        [Test] public void ReliableAction_DoInvoke_OnlyOnce()
        {
            var storage = Substitute.For<IReliableActionsStorage>();
            var fallbackInvoker = Substitute.For<IFallbackInvoker>();
            var reliableAction = new TestReliableAction(IncrementCallsCount, storage, fallbackInvoker);

            reliableAction.TryInvoke();
            reliableAction.TryInvoke();

            _methodCallsCount.Should().Be(1);
        }

        [Test] public void ReliableAction_IsInvokedProperty_ReturnsTrue_AfterSuccessfullyInvoked()
        {
            var storage = Substitute.For<IReliableActionsStorage>();
            var fallbackInvoker = Substitute.For<IFallbackInvoker>();
            var reliableAction = new TestReliableAction(IncrementCallsCount, storage, fallbackInvoker);

            reliableAction.TryInvoke();

            reliableAction.IsInvoked.Should().Be(true);
        }

        [Test] public void ReliableAction_TryInvokeMethod_ReturnsTrue_WhenSuccessfullyInvoked()
        {
            var storage = Substitute.For<IReliableActionsStorage>();
            var fallbackInvoker = Substitute.For<IFallbackInvoker>();
            var reliableAction = new TestReliableAction(IncrementCallsCount, storage, fallbackInvoker);

            var isInvoked = reliableAction.TryInvoke();

            isInvoked.Should().Be(true);
        }

        [Test] public void ReliableAction_DoesNotInvokes_AfterCancellation()
        {
            var storage = Substitute.For<IReliableActionsStorage>();
            var fallbackInvoker = Substitute.For<IFallbackInvoker>();
            var reliableAction = new TestReliableAction(IncrementCallsCount, storage, fallbackInvoker);

            reliableAction.Cancel();
            reliableAction.TryInvoke();

            _methodCallsCount.Should().Be(0);
        }

        [Test] public void ReliableAction_TryInvoke_ReturnsFalse_WhenInvocationCancelled()
        {
            var storage = Substitute.For<IReliableActionsStorage>();
            var fallbackInvoker = Substitute.For<IFallbackInvoker>();
            var reliableAction = new TestReliableAction(IncrementCallsCount, storage, fallbackInvoker);

            reliableAction.Cancel();
            var isInvoked = reliableAction.TryInvoke();

            isInvoked.Should().Be(false);
        }

        [Test] public void ReliableAction_IsCancelledProperty_ReturnsTrue_WhenInvocationCancelled()
        {
            var storage = Substitute.For<IReliableActionsStorage>();
            var fallbackInvoker = Substitute.For<IFallbackInvoker>();
            var reliableAction = new TestReliableAction(IncrementCallsCount, storage, fallbackInvoker);

            reliableAction.Cancel();

            reliableAction.IsCancelled.Should().Be(true);
        }

        [Test] public void ReliableAction_DoesNotInvokes_WithLockedInvocation()
        {
            var storage = Substitute.For<IReliableActionsStorage>();
            var fallbackInvoker = Substitute.For<IFallbackInvoker>();
            var reliableAction = new TestReliableAction(IncrementCallsCount, storage, fallbackInvoker);

            var _ = reliableAction.LockInvocation();
            reliableAction.TryInvoke();

            _methodCallsCount.Should().Be(0);
        }

        [Test] public void ReliableAction_IsLockedProperty_ReturnsTrue_WithLockedInvocation()
        {
            var storage = Substitute.For<IReliableActionsStorage>();
            var fallbackInvoker = Substitute.For<IFallbackInvoker>();
            var reliableAction = new TestReliableAction(IncrementCallsCount, storage, fallbackInvoker);

            var _ = reliableAction.LockInvocation();

            reliableAction.IsLocked.Should().Be(true);
        }

        private void IncrementCallsCount() => _methodCallsCount++;
    }

    internal class TestReliableAction : BaseReliableAction
    {
        public TestReliableAction(Action action, IReliableActionsStorage storage, IFallbackInvoker invoker, bool isFallbackInvocation = false) 
            : base(storage, invoker, isFallbackInvocation)
        {
            _action = action;
        }

        private readonly Action _action;
        public override Guid TypeGuid => new("C3B7E643-9358-4FCF-9337-9BA6403F1F11");

        public override void Save(string saveKey) => throw new NotImplementedException();
        public override void Load(string saveKey) => throw new NotImplementedException();
        public override void DeleteSave(string saveKey) => throw new NotImplementedException();

        protected override void Invoke()
        {
            _action.Invoke();
        }
    }

    // TODO: Test this too
    internal class TestFallbackInvoker { }
}