using System;
using NUnit.Framework;
using UnityUtils.Invocation;

namespace Invocation
{
    public class DisposableActionTests
    {
        [Test] public void DisposableAction_Fires_AfterHandleReleased()
        {
            int callCount = 0;
            void MyAction() => callCount++;

            var disposableAction = new DisposableAction(MyAction);
            disposableAction.Dispose();
            
            Assert.AreEqual(callCount, 1);
        }
        
        [Test] public void DisposableAction_DontFires_WhenHandleDontReleased()
        {
            int callCount = 0;
            void MyAction() => callCount++;

            var disposableAction = new DisposableAction(MyAction);
            
            Assert.AreEqual(callCount, 0);
        }

        [Test] public void DisposableAction_Fires_WithCorrectArguments()
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

            int passedInt = 5;
            float passedFloat = 6.3f;
            string passedString = "asdf";
            var disposableAction = new DisposableAction<Action<int, float, string>>(MyAction, passedInt, passedFloat, passedString);
            disposableAction.Dispose();
            
            Assert.AreEqual(passedInt, receivedInt);
            Assert.AreEqual(passedFloat, receivedFloat);
            Assert.AreEqual(passedString, receivedString);
        }
    }
}