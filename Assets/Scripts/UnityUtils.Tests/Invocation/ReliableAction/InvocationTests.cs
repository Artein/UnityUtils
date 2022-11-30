using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using UnityUtils.Invocation.ReliableAction;

namespace Invocation.ReliableAction
{
    [TestFixture]
    public class InvocationTests
    {
        private int _methodCallsCount;
        private IReliableActionsStorage _storage;
        private IFallbackInvoker _fallbackInvoker;

        [SetUp] public void Setup()
        {
            _methodCallsCount = 0;
            _storage = Substitute.For<IReliableActionsStorage>();
            _fallbackInvoker = Substitute.For<IFallbackInvoker>();
        }

        [Test] public void ReliableAction_Invokes()
        {
            var reliableAction = new TestsReliableAction(IncrementCallsCount, _storage, _fallbackInvoker);

            reliableAction.TryInvoke();

            _methodCallsCount.Should().Be(1);
        }

        [Test] public void ReliableAction_DoInvoke_OnlyOnce()
        {
            var reliableAction = new TestsReliableAction(IncrementCallsCount, _storage, _fallbackInvoker);

            reliableAction.TryInvoke();
            reliableAction.TryInvoke();

            _methodCallsCount.Should().Be(1);
        }

        [Test] public void ReliableAction_IsInvokedProperty_ReturnsTrue_AfterSuccessfullyInvoked()
        {
            var reliableAction = new TestsReliableAction(IncrementCallsCount, _storage, _fallbackInvoker);

            reliableAction.TryInvoke();

            reliableAction.IsInvoked.Should().Be(true);
        }

        [Test] public void ReliableAction_TryInvokeMethod_ReturnsTrue_WhenSuccessfullyInvoked()
        {
            var reliableAction = new TestsReliableAction(IncrementCallsCount, _storage, _fallbackInvoker);

            var isInvoked = reliableAction.TryInvoke();

            isInvoked.Should().Be(true);
        }

        [Test] public void ReliableAction_DoesNotInvokes_AfterCancellation()
        {
            var reliableAction = new TestsReliableAction(IncrementCallsCount, _storage, _fallbackInvoker);

            reliableAction.Cancel();
            reliableAction.TryInvoke();

            _methodCallsCount.Should().Be(0);
        }

        [Test] public void ReliableAction_TryInvoke_ReturnsFalse_WhenInvocationCancelled()
        {
            var reliableAction = new TestsReliableAction(IncrementCallsCount, _storage, _fallbackInvoker);

            reliableAction.Cancel();
            var isInvoked = reliableAction.TryInvoke();

            isInvoked.Should().Be(false);
        }

        [Test] public void ReliableAction_IsCancelledProperty_ReturnsTrue_WhenInvocationCancelled()
        {
            var reliableAction = new TestsReliableAction(IncrementCallsCount, _storage, _fallbackInvoker);

            reliableAction.Cancel();

            reliableAction.IsCancelled.Should().Be(true);
        }

        [Test] public void ReliableAction_DoesNotInvokes_WithLockedInvocation()
        {
            var reliableAction = new TestsReliableAction(IncrementCallsCount, _storage, _fallbackInvoker);

            var _ = reliableAction.LockInvocation();
            reliableAction.TryInvoke();

            _methodCallsCount.Should().Be(0);
        }

        [Test] public void ReliableAction_IsLockedProperty_ReturnsTrue_WithLockedInvocation()
        {
            var reliableAction = new TestsReliableAction(IncrementCallsCount, _storage, _fallbackInvoker);

            var _ = reliableAction.LockInvocation();

            reliableAction.IsLocked.Should().Be(true);
        }

        private void IncrementCallsCount() => _methodCallsCount++;
    }
}