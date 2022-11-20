using UnityUtils.Invocation;

namespace UnityUtils.Examples.Invocation
{
    public class OrderedEventExamples
    {
        public OrderedEventExamples()
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