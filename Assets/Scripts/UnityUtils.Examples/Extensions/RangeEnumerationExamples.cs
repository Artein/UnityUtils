using System;
using UnityUtils.Extensions;

namespace UnityUtils.Examples.Extensions
{
    internal class RangeEnumerationExamples
    {
        public RangeEnumerationExamples()
        {
            // 1. Fixed range with variable as right bound. Prints from [5; 20]
            int length = 20;
            foreach (var i in 5..length)
            {
                UnityEngine.Debug.Log(i);
            }
            
            // 2. Backward range. Prints from [10; 5]
            foreach (var i in 10..5)
            {
                UnityEngine.Debug.Log(i);
            }
            
            // 3. Left bound open range. Starts always from 0. Prints from [0; 10]
            foreach (var i in ..10)
            {
                UnityEngine.Debug.Log(i);
            }
            
            // 4. Right bound open range. Throws NotSupportedException
            try
            {
                foreach (var i in 5..)
                {
                    UnityEngine.Debug.Log(i);
                }
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            // 5. Fixed indexer. Prints from [0; 10]
            foreach (var i in 10)
            {
                UnityEngine.Debug.Log(i);
            }
        }
    }
}