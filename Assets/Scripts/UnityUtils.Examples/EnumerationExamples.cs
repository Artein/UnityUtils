using System;
using UnityUtils.Enumeration;

namespace UnityUtils.Examples
{
    public class EnumerationExamples
    {
        void Enumerate()
        {
            // 1. Fixed range. Prints from [0; 10]
            foreach (var i in 0..10)
            {
                UnityEngine.Debug.Log(i);
            }
            
            // 2. Prints from [5; 10]
            foreach (var i in 5..10)
            {
                UnityEngine.Debug.Log(i);
            }
            
            // 3. Supports variables. Prints from [5; 20]
            int length = 20;
            foreach (var i in 5..length)
            {
                UnityEngine.Debug.Log(i);
            }
            
            // 4. Range can be open. Prints from [0; 10]
            foreach (var i in ..10)
            {
                UnityEngine.Debug.Log(i);
            }
            
            // 5. Not closed range throws NotSupportedException
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
            
            // 6. Fixed counter. Prints from [0; 9]
            foreach (var i in 10)
            {
                UnityEngine.Debug.Log(i);
            }
        }
    }
}