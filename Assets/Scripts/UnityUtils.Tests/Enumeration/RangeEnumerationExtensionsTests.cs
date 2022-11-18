using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityUtils.Enumeration;

namespace Enumeration
{
    public class RangeEnumerationExtensionsTests
    {
        [Test] public void FixedIndexer()
        {
            var results = new List<int>();
            foreach (var i in 2)
            {
                results.Add(i);
            }

            Assert.AreEqual(results, new List<int>{ 0, 1, 2 });
        }
        
        [Test] public void FixedRange()
        {
            var results = new List<int>();
            foreach (var i in 0..2)
            {
                results.Add(i);
            }

            Assert.AreEqual(results, new List<int>{ 0, 1, 2 });
        }

        [Test] public void OpenRange()
        {
            var results = new List<int>();
            foreach (int i in ..2)
            {
                results.Add(i);
            }
            
            Assert.AreEqual(results, new List<int>{ 0, 1, 2 });
        }

        [Test] public void InfiniteRangeThrowsException()
        {
            try
            {
                foreach (int i in 1..) { }
                Assert.Fail();
            }
            catch (NotSupportedException exception)
            {
                Assert.AreEqual(exception.Message, "Range must be closed");
            }
        }
        
        [Test] public void BackwardRange()
        {
            var results = new List<int>();
            foreach (int i in 2..0)
            {
                results.Add(i);
            }
            
            Assert.AreEqual(results, new List<int>{ 2, 1, 0 });
        }
    }
}