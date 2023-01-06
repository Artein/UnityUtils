using FluentAssertions;
using NUnit.Framework;
using UnityUtils.State.Locking;

namespace State
{
    [TestFixture] public class LockerTests
    {
        [Test] public void LockedByDefault()
        {
            var locker = new Locker();

            locker.IsLocked.Should().Be(true);
        }
        
        [Test] public void Unlocked_Setting_LockedByDefault_ToFalse()
        {
            var locker = new Locker(false);

            locker.IsLocked.Should().Be(false);
        }
        
        [Test] public void Locked_AfterLockIsCalled()
        {
            var locker = new Locker(false);
            var _ = locker.Lock();

            locker.IsLocked.Should().Be(true);
        }
        
        [Test] public void Unlocked_AfterLockHandle_IsReleased()
        {
            var locker = new Locker(false);
            var lockHandle = locker.Lock();
            lockHandle.Dispose();

            locker.IsLocked.Should().Be(false);
        }
        
        [Test] public void Locked_WhileAtLeastOneLockHandle_IsNotReleased()
        {
            var locker = new Locker(false);
            var lockHandle = locker.Lock();
            var _ = locker.Lock();
            lockHandle.Dispose();

            locker.IsLocked.Should().Be(true);
        }
        
        [Test] public void After_Reset_MimicsCreationState_WithBeingLockedByDefault()
        {
            var locker = new Locker(true);
            locker.Dispose(); // unlock
            locker.Reset();

            locker.IsLocked.Should().Be(true);
        }
        
        [Test] public void After_Reset_MimicsCreationState_WithBeingUnlockedByDefault()
        {
            var locker = new Locker(false);
            locker.Dispose();
            locker.Reset();

            locker.IsLocked.Should().Be(false);
        }
        
        [Test] public void Reset_SetsLockerToANewState_EvenThoughThereIsALockFromPreviousState()
        {
            var locker = new Locker(false);
            var lockHandleFromPreviousState = locker.Lock();
            locker.Reset();

            locker.IsLocked.Should().Be(false);
        }
        
        [Test] public void ReleaseLockHandle_FromPreviousState_DoesNotImpact_CurrentState()
        {
            var locker = new Locker(true);
            var lockHandleFromPreviousState = locker.Lock();
            locker.Dispose();
            locker.Reset();
            lockHandleFromPreviousState.Dispose();

            locker.IsLocked.Should().Be(true);
        }
    }
}