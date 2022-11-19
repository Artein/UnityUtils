using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityUtils.Invocation
{
    public class OrderedEvent
    {
        private readonly List<KeyValuePair<int, Action>> _orderedActions = new();

        [MustUseReturnValue("Dispose handle to unsubscribe")]
        public IDisposable Subscribe(int order, Action action)
        {
            var pair = new KeyValuePair<int, Action>(order, action);
            _orderedActions.Add(pair);
            // Performance: based on the use case we can also sort right before firing event
            // Depends on subscription amount to firing amount
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
        
        private class Comparator : IComparer<KeyValuePair<int, Action>>
        {
            public static readonly Comparator Default = new();
                
            public int Compare(KeyValuePair<int, Action> left, KeyValuePair<int, Action> right)
            {
                return left.Key.CompareTo(right.Value);
            }
        }
    }
}