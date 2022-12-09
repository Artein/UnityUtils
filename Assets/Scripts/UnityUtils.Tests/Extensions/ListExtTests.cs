using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using UnityUtils.Extensions;
using Is = NUnit.Framework.Is;

namespace Extensions
{
    [TestFixture] public class ListExtTests
    {
        [Test] public void Reverse_NoHeapAlloc_Method_SetListReversed()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            
            list.Reverse_NoHeapAlloc();

            list.Should().ContainInOrder(5, 4, 3, 2, 1);
        }
        
        [Test] public void Reverse_NoHeapAlloc_Method_DoesNotProduceHeapAllocations()
        {
            var list = Enumerable.Repeat(1, 10).ToList();
            
            Assert.That(list.Reverse_NoHeapAlloc, Is.Not.AllocatingGCMemory());
        }
    }
}