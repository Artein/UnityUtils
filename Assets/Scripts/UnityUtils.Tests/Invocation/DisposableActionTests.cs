using System;
using FluentAssertions;
using NUnit.Framework;
using UnityUtils.Invocation;

namespace Invocation
{
    public class DisposableActionTests
    {
        [Test] public void Fires_AfterHandleReleased()
        {
            int callCount = 0;
            void MyAction() => callCount++;

            IDisposable disposableAction = new DisposableAction(MyAction);
            disposableAction.Dispose();

            callCount.Should().Be(1);
        }
        
        [Test] public void DontFires_WhenHandleWasntReleased()
        {
            int callCount = 0;
            void MyAction() => callCount++;

            var _ = new DisposableAction(MyAction);

            callCount.Should().Be(0);
        }
        
        [Test] public void WithArguments_Fires_AfterHandleReleased()
        {
            int callCount = 0;
            void MyAction(Args _) => callCount++;

            IDisposable disposableAction = new DisposableAction<Args>(MyAction, new Args());
            disposableAction.Dispose();

            callCount.Should().Be(1);
        }
        
        [Test] public void WithArguments_DontFires_WhenHandleWasntReleased()
        {
            int callCount = 0;
            void MyAction(Args _) => callCount++;

            var _ = new DisposableAction<Args>(MyAction, new Args());

            callCount.Should().Be(0);
        }

        [Test] public void WithArguments_Fires_HavingCorrectArguments()
        {
            var receivedArgs = new Args();
            
            void MyAction(Args args)
            {
                receivedArgs = args;
            }

            var passedArgs = new Args { Int = 5, Float = 6.3f, String = "asdf" };
            IDisposable disposableAction = new DisposableAction<Args>(MyAction, passedArgs);
            disposableAction.Dispose();

            receivedArgs.Int.Should().Be(passedArgs.Int);
            receivedArgs.Float.Should().Be(passedArgs.Float);
            receivedArgs.String.Should().Be(passedArgs.String);
        }

        private struct Args
        {
            public int Int;
            public float Float;
            public string String;
        }
    }
}