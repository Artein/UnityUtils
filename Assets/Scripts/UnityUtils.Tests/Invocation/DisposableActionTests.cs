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

            var disposableAction = new DisposableAction(MyAction);
            disposableAction.Dispose();

            callCount.Should().Be(1);
        }
        
        [Test] public void DontFires_WhenHandleWasntReleased()
        {
            int callCount = 0;
            void MyAction() => callCount++;

            var disposableAction = new DisposableAction(MyAction);

            callCount.Should().Be(0);
        }

        [Test] public void Fires_WithCorrectArguments()
        {
            int receivedInt = int.MinValue;
            float receivedFloat = float.MinValue;
            string receivedString = null;
            
            void MyAction(int arg1, float arg2, string arg3)
            {
                receivedInt = arg1;
                receivedFloat = arg2;
                receivedString = arg3;
            }

            const int passedInt = 5;
            const float passedFloat = 6.3f;
            const string passedString = "asdf";
            var disposableAction = new DisposableAction<Action<int, float, string>>(MyAction, passedInt, passedFloat, passedString);
            disposableAction.Dispose();

            receivedInt.Should().Be(passedInt);
            receivedFloat.Should().Be(passedFloat);
            receivedString.Should().Be(passedString);
        }
    }
}