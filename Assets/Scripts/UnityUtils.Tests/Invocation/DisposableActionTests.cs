using System;
using FluentAssertions;
using NUnit.Framework;
using UnityUtils.Invocation;

namespace Invocation
{
    [TestFixture] public class DisposableActionTests
    {
        [Test] public void Fires_AfterHandleReleased()
        {
            int callCount = 0;
            void MyAction() => callCount++;

            IDisposable disposableAction = new DisposableAction(MyAction);
            disposableAction.Dispose();

            callCount.Should().Be(1);
        }
        
        [Test] public void DontFires_WhenHandleWasNotReleased()
        {
            int callCount = 0;
            void MyAction() => callCount++;

            var _ = new DisposableAction(MyAction);

            callCount.Should().Be(0);
        }

        [Test] public void CanBeReused_AfterReset()
        {
            int callCount = 0;
            void MyAction() => callCount++;
            
            var disposableAction = new DisposableAction(MyAction);
            disposableAction.Dispose();
            disposableAction.Reset();
            disposableAction.Dispose();
            
            callCount.Should().Be(2);
        }
        
        [Test] public void CanBeFired_MultipleTimes()
        {
            int callCount = 0;
            void MyAction() => callCount++;
            
            IDisposable disposableAction = new DisposableAction(MyAction, false);
            disposableAction.Dispose();
            disposableAction.Dispose();
            disposableAction.Dispose();
            
            callCount.Should().Be(3);
        }

        [Test] public void WithArguments_Fires_AfterHandleReleased()
        {
            int callCount = 0;
            void MyAction(Args _) => callCount++;

            IDisposable disposableAction = new DisposableAction<Args>(MyAction, new Args());
            disposableAction.Dispose();

            callCount.Should().Be(1);
        }

        [Test] public void WithArguments_DontFires_WhenHandleWasNotReleased()
        {
            int callCount = 0;
            void MyAction(Args _) => callCount++;

            var _ = new DisposableAction<Args>(MyAction, new Args());

            callCount.Should().Be(0);
        }

        [Test] public void WithArguments_Fires_HavingCorrectArguments()
        {
            var receivedArgs = new Args();
            void MyAction(Args args) => receivedArgs = args;

            var passedArgs = new Args { Int = 5, Float = 6.3f, String = "MyString" };
            IDisposable disposableAction = new DisposableAction<Args>(MyAction, passedArgs);
            disposableAction.Dispose();

            receivedArgs.Int.Should().Be(passedArgs.Int);
            receivedArgs.Float.Should().Be(passedArgs.Float);
            receivedArgs.String.Should().Be(passedArgs.String);
        }
        
        [Test] public void WithArguments_CanBeReused_AfterReset()
        {
            int callCount = 0;
            void MyAction(Args _) => callCount++;

            var disposableAction = new DisposableAction<Args>(MyAction, new Args());
            disposableAction.Dispose();
            disposableAction.Reset();
            disposableAction.Dispose();
            
            callCount.Should().Be(2);
        }
        
        [Test] public void WithArguments_CanBeFired_MultipleTimes()
        {
            int callCount = 0;
            void MyAction(Args _) => callCount++;

            IDisposable disposableAction = new DisposableAction<Args>(MyAction, new Args(), false);
            disposableAction.Dispose();
            disposableAction.Dispose();
            disposableAction.Dispose();
            
            callCount.Should().Be(3);
        }

        private struct Args
        {
            public int Int;
            public float Float;
            public string String;
        }
    }
}