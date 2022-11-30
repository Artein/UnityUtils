using System;
using FluentAssertions;
using NUnit.Framework;
using UnityUtils.Invocation;

namespace Invocation
{
    [TestFixture]
    public class DeferredInvocationTests
    {
        [Test] public void LockedAtCreation()
        {
            int callsCount = 0;
            void SomeAction() => callsCount++;
            var _ = new DeferredInvocation(SomeAction);

            callsCount.Should().Be(0);
        }

        [Test] public void Invokes_WhenNoAdditionalLocks()
        {
            int callsCount = 0;
            void SomeAction() => callsCount++;
            var di = new DeferredInvocation(SomeAction);
            
            di.Dispose();

            callsCount.Should().Be(1);
        }

        [Test] public void Invokes_Once_WithMultipleDisposeCalls()
        {
            int callsCount = 0;
            void SomeAction() => callsCount++;
            var di = new DeferredInvocation(SomeAction);
            
            di.Dispose();
            di.Dispose();

            callsCount.Should().Be(1);
        }

        [Test] public void DoesNotInvokes_Until_AllLocksReleased()
        {
            int callsCount = 0;
            void SomeAction() => callsCount++;
            void DIAdditionalLock(IDeferredInvocation di) { var _ = di.LockInvocation(); }
            
            using (var di = new DeferredInvocation(SomeAction))
            {
                DIAdditionalLock(di);
            }
            
            callsCount.Should().Be(0);
        }

        [Test] public void Invokes_PreviouslyHavingMultipleLocks()
        {
            int callsCount = 0;
            IDisposable additionalLockHandle;
            void SomeAction() => callsCount++;
            void DIAdditionalLock(IDeferredInvocation di) => additionalLockHandle = di.LockInvocation();
            void UnlockDIAdditionalLock() => additionalLockHandle.Dispose();
            
            using (var di = new DeferredInvocation(SomeAction))
            {
                DIAdditionalLock(di);
            }
            UnlockDIAdditionalLock();

            callsCount.Should().Be(1);
        }

        [Test] public void Invocation_Cancels()
        {
            int callsCount = 0;
            void SomeAction() => callsCount++;
            
            var di = new DeferredInvocation(SomeAction);
            di.Cancel();

            callsCount.Should().Be(0);
        }
        
        [Test] public void Invocation_Cancels_HavingMultipleLocks()
        {
            int callsCount = 0;
            IDisposable additionalLockHandle;
            void SomeAction() => callsCount++;
            void DIAdditionalLock(IDeferredInvocation di) => additionalLockHandle = di.LockInvocation();
            void UnlockDIAdditionalLock() => additionalLockHandle.Dispose();
            
            using (var di = new DeferredInvocation(SomeAction))
            {
                DIAdditionalLock(di);
                di.Cancel();
                UnlockDIAdditionalLock();
            }

            callsCount.Should().Be(0);
        }

        [Test] public void Invocation_PassingDisposableAction()
        {
            const int passedInt = 5;
            var receivedArgs = new Args();
            void SomeAction(Args args) => receivedArgs = args;
            
            var di = new DeferredInvocation(new DisposableAction<Args>(SomeAction, new Args { Value = passedInt }));
            di.Dispose();

            receivedArgs.Value.Should().Be(passedInt);
        }

        private struct Args
        {
            public int Value;
        }
    }
}