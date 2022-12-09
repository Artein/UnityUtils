using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using UnityUtils.Invocation;

namespace Invocation
{
    [TestFixture] public class OrderedEventTests
    {
        [Test] public void Fires()
        {
            int callCount = 0;
            void LocalFunction() => callCount++;
            var orderedEvent = new OrderedEvent();
            using var handle = orderedEvent.Subscribe(0, LocalFunction);
            
            orderedEvent.Fire();

            callCount.Should().Be(1);
        }

        [Test] public void NoExceptions_WhenFires_WithoutSubscribers()
        {
            var orderedEvent = new OrderedEvent();
            Action action = () => orderedEvent.Fire();

            action.Should().NotThrow();
        }

        [Test] public void WhenFires_SubscribersReceiveEvents_InRightOrder()
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

            receivedEvents.Should().Equal(new List<int> { 1, 2, 3, 4 });
        }

        [Test] public void AfterReceiverUnsubscribed_Fires_WithoutSendingEvent_ToUnsubscribedReceiver()
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

            receivedEvents.Should().Equal(new List<int> { 1, 2, 4 });
        }
    }
}