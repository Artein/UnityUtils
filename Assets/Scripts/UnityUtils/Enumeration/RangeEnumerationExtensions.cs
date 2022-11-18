using System;

namespace UnityUtils.Enumeration
{
    // Makes possible to enumerate with ranges. No heap allocations. Check EnumerationExamples for examples
    public static class RangeEnumerationExtensions
    {
        public static IntEnumerator GetEnumerator(this Range range)
        {
            if (range.End.IsFromEnd)
            {
                throw new NotSupportedException("Range must be closed");
            }
            
            return new IntEnumerator(range);
        }

        public static IntEnumerator GetEnumerator(this int count)
        {
            return new IntEnumerator(new Range(0, count));
        }

        public ref struct IntEnumerator
        {
            private readonly int _end;
            private readonly int _step;
            
            public IntEnumerator(Range range)
            {
                _step = range.Start.Value <= range.End.Value ? 1 : -1;
                Current = range.Start.Value - _step;
                _end = range.End.Value;
            }

            public int Current { get; private set; }

            public bool MoveNext()
            {
                Current += _step;
                return Current * _step <= _end * _step;
            }
        }
    }
}