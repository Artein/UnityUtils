using System;

namespace UnityUtils.Enumeration
{
    // Makes possible to enumerate with ranges. No heap allocations. Check EnumerationExamples for examples
    public static class RangeEnumerationExtensions
    {
        public static CustomIntEnumerator GetEnumerator(this Range range)
        {
            return new CustomIntEnumerator(range);
        }

        public static CustomIntEnumerator GetEnumerator(this int count)
        {
            return new CustomIntEnumerator(new Range(0, count - 1));
        }

        public ref struct CustomIntEnumerator
        {
            private readonly int _end;
            
            public CustomIntEnumerator(Range range)
            {
                if (range.End.IsFromEnd)
                {
                    throw new NotSupportedException("Range must be closed");
                }
                
                Current = range.Start.Value - 1;
                _end = range.End.Value;
            }

            public int Current { get; private set; }

            public bool MoveNext()
            {
                Current++;
                return Current <= _end;
            }
        }
    }
}