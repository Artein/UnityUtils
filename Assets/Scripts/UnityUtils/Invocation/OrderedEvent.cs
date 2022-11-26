using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace UnityUtils.Invocation
{
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    [DebuggerDisplay("SubscribersCount: {_orderedActions.Count}")]
    public class OrderedEvent
    {
        private readonly List<KeyValuePair<int, Action>> _orderedActions = new();

        [MustUseReturnValue("Dispose handle to unsubscribe")]
        public IDisposable Subscribe(int order, Action action)
        {
            var pair = new KeyValuePair<int, Action>(order, action);
            _orderedActions.Add(pair);
            // Performance: based on the use case we can also sort right before firing event
            // Depends on subscription amount and firing amount
            _orderedActions.Sort(Comparator.Default);

            return new DisposableAction<KeyValuePair<int, Action>>(Remove, pair);
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

        private void Remove(KeyValuePair<int, Action> pair)
        {
            _orderedActions.Remove(pair);
        }

        private class Comparator : IComparer<KeyValuePair<int, Action>>
        {
            public static readonly Comparator Default = new();

            public int Compare(KeyValuePair<int, Action> left, KeyValuePair<int, Action> right)
            {
                return left.Key.CompareTo(right.Key);
            }
        }

#region DebuggerProxy
        private class DebuggerProxy
        {
            private readonly OrderedEvent _orderedEvent;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public KeyValuePair<int, string>[] Values
            {
                get
                {
                    var result = new KeyValuePair<int, string>[_orderedEvent._orderedActions.Count];
                    for (int i = 0; i < _orderedEvent._orderedActions.Count; i++)
                    {
                        var (order, action) = _orderedEvent._orderedActions[i];
                        result[i] = new KeyValuePair<int, string>(order, $"'{action.Target}___{action.Method}'");
                    }

                    return result;
                }
            }

            public DebuggerProxy(OrderedEvent orderedEvent)
            {
                _orderedEvent = orderedEvent;
            }
        }
#endregion DebuggerProxy
    }
}