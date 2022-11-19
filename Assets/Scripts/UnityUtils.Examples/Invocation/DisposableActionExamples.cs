using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityUtils.Invocation;

namespace UnityUtils.Examples.Invocation
{
    // 1. Idea is to store some number. Storage is encapsulated and nobody without handle owner can remove the number
    internal class NumbersStoringExample
    {
        private class NumbersStorage
        {
            private readonly List<int> _numbers = new();
            
            [MustUseReturnValue]
            public IDisposable Store(int number)
            {
                _numbers.Add(number);
                return new DisposableAction<Action<int>>(Remove, number);
            }

            private void Remove(int number)
            {
                _numbers.Remove(number);
            }
        }

        public NumbersStoringExample()
        {
            var numbersStorage = new NumbersStorage();

            // the number will be in Storage up until handle is disposed
            var handle = numbersStorage.Store(10);

            // handle might be safely hidden under using statement. In this case storage is up until closing bracket
            using (numbersStorage.Store(5))
            {
            }
            
            handle.Dispose();
        }
    }
    
    // 2. Implementation of ordered subscription to event. Subscribers receive event based on passed order
    internal class OrderedEventExample
    {
        private class OrderedEvent
        {
            private class Comparator : IComparer<KeyValuePair<int, Action>>
            {
                public static readonly Comparator Default = new();
                
                public int Compare(KeyValuePair<int, Action> left, KeyValuePair<int, Action> right)
                {
                    return left.Key.CompareTo(right.Value);
                }
            }

            private readonly List<KeyValuePair<int, Action>> _orderedActions = new();

            [MustUseReturnValue("Dispose handle to unsubscribe")]
            public IDisposable Subscribe(int order, Action action)
            {
                var pair = new KeyValuePair<int, Action>(order, action);
                _orderedActions.Add(pair);
                // Performance: based on the use case we can also sort right before firing event
                _orderedActions.Sort(Comparator.Default);

                return new DisposableAction(() =>
                {
                    _orderedActions.Remove(pair);
                });
            }

            public void Fire()
            {
                for (int i = 0; i < _orderedActions.Count; i++)
                {
                    var pair = _orderedActions[i];
                    var action = pair.Value;
                    action.Invoke();
                }
            }
        }

        public OrderedEventExample()
        {
            var myEvent = new OrderedEvent();

            // subscription order is 3 -> 2 -> 1 -> 4
            var thirdSubscriptionHandle = myEvent.Subscribe(3, Third);
            var secondSubscriptionHandle = myEvent.Subscribe(2, Second);
            var firstSubscriptionHandle = myEvent.Subscribe(1, First);
            var fourthSubscriptionHandle = myEvent.Subscribe(4, Fourth);
            
            // call will be 1 -> 2 -> 3 -> 4
            myEvent.Fire();
            
            // for some reason we don't need 2nd call
            secondSubscriptionHandle.Dispose();
            
            // call will be 1 -> 3 -> 4
            myEvent.Fire();
            
            // safely release rest of subscriptions
            firstSubscriptionHandle.Dispose();
            thirdSubscriptionHandle.Dispose();
            fourthSubscriptionHandle.Dispose();
        }

        private void First()
        {
            UnityEngine.Debug.Log($"{nameof(First)}");
        }
        
        private void Second()
        {
            UnityEngine.Debug.Log($"{nameof(Second)}");
        }
        
        private void Third()
        {
            UnityEngine.Debug.Log($"{nameof(Third)}");
        }
        
        private void Fourth()
        {
            UnityEngine.Debug.Log($"{nameof(Fourth)}");
        }
    }
}