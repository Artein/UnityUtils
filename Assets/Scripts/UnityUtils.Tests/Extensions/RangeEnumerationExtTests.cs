using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using UnityUtils.Extensions;

namespace Extensions
{
    [TestFixture] public class RangeEnumerationExtTests
    {
        [Test] public void FixedIndexer()
        {
            var results = new List<int>();
            foreach (var i in 2) // [0; 2]
            {
                results.Add(i);
            }

            results.Should().Equal(new List<int> { 0, 1, 2 });
        }
        
        [Test] public void FixedRange()
        {
            var results = new List<int>();
            foreach (var i in 0..2)
            {
                results.Add(i);
            }

            results.Should().Equal(new List<int> { 0, 1, 2 });
        }

        [Test] public void OpenRange()
        {
            var results = new List<int>();
            foreach (int i in ..2)
            {
                results.Add(i);
            }
            
            results.Should().Equal(new List<int> { 0, 1, 2 });
        }

        [Test] public void InfiniteRangeThrowsException()
        {
            Action action = () => { foreach (int _ in 1..) { } };

            action.Should().Throw<NotSupportedException>().WithMessage("Range must be closed");
        }
        
        [Test] public void BackwardRange()
        {
            var results = new List<int>();
            foreach (int i in 2..0)
            {
                results.Add(i);
            }
            
            results.Should().Equal(new List<int> { 2, 1, 0 });
        }
        
        [Test] public void SingleRange()
        {
            var results = new List<int>();
            foreach (int i in 2..2)
            {
                results.Add(i);
            }
            
            results.Should().Equal(new List<int> { 2 });
        }
    }
}