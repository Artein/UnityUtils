using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityUtils.Invocation;

namespace UnityUtils.Examples.Invocation
{
    // 1. Idea is to store some number. Storage is encapsulated and only handle owner can remove the number
    internal class NumbersStoringExample
    {
        private class NumbersStorage
        {
            private readonly List<int> _numbers = new();
            
            [MustUseReturnValue]
            public IDisposable Store(int number)
            {
                _numbers.Add(number);
                return new DisposableAction<int>(Remove, number);
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
}