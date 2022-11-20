using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityUtils.Invocation;

namespace Invocation
{
    public class OrderedEventTests
    {
        [Test] public void OrderedEvent_Fires()
        {
            int callCount = 0;
            void LocalFunction() => callCount++;
            var orderedEvent = new OrderedEvent();
            using var handle = orderedEvent.Subscribe(0, LocalFunction);
            
            orderedEvent.Fire();

            Assert.AreEqual(1, callCount);
        }

        [Test] public void OrderedEvent_NoExceptions_WhenFires_WithoutSubscribers()
        {
            try
            {
                var orderedEvent = new OrderedEvent();
                orderedEvent.Fire();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test] public void OrderedEvent_WhenFires_SubscribersReceiveEvents_InRightOrder()
        {
            var receivedEvents = new List<int>();
            void LocalFunction1() => receivedEvents.Add(1);
            void LocalFunction2() => receivedEvents.Add(2);
            void LocalFunction3() => receivedEvents.Add(3);
            void LocalFunction4() => receivedEvents.Add(4);
            var orderedEvent = new OrderedEvent();
            using var handle1 = orderedEvent.Subscribe(1, LocalFunction1);
            using var handle3 = orderedEvent.Subscribe(3, LocalFunction3);
            using var handle4 = orderedEvent.Subscribe(4, LocalFunction4);
            using var handle2 = orderedEvent.Subscribe(2, LocalFunction2);
            
            orderedEvent.Fire();
            
            Assert.AreEqual(new List<int>{ 1, 2, 3, 4 }, receivedEvents);
        }

        [Test] public void OrderedEvent_AfterReceiverUnsubscribed_Fires_WithoutSendingEvent_ToUnsubscribedReceiver()
        {
            var receivedEvents = new List<int>();
            void LocalFunction1() => receivedEvents.Add(1);
            void LocalFunction2() => receivedEvents.Add(2);
            void LocalFunction3() => receivedEvents.Add(3);
            void LocalFunction4() => receivedEvents.Add(4);
            var orderedEvent = new OrderedEvent();
            using var handle1 = orderedEvent.Subscribe(1, LocalFunction1);
            var handle3 = orderedEvent.Subscribe(3, LocalFunction3);
            using var handle4 = orderedEvent.Subscribe(4, LocalFunction4);
            using var handle2 = orderedEvent.Subscribe(2, LocalFunction2);
            
            handle3.Dispose();
            orderedEvent.Fire();
            
            Assert.AreEqual(new List<int>{ 1, 2, 4 }, receivedEvents);
        }
    }
}