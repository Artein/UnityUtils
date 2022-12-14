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

        [Test] public void EnsureSize_IncreasesSize()
        {
            const int initialCapacity = 10;
            const int requiredSize = initialCapacity + 1;
            var list = new List<int>(initialCapacity);

            list.EnsureSize(requiredSize);

            list.Count.Should().Be(requiredSize);
        }

        [Test] public void EnsureSize_FillsWithProvidedValue()
        {
            var list = new List<int> { 1, 2, 3 };

            list.EnsureSize(6, 5);

            list.Should().ContainInOrder(1, 2, 3, 5, 5, 5);
        }
    }
}