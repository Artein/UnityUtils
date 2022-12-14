using System;

namespace UnityUtils.Extensions
{
    // Makes possible to enumerate with ranges. Check EnumerationExamples for examples
    public static class RangeEnumerationExt
    {
        public static IntEnumerator GetEnumerator(this Range range)
        {
            if (range.End.IsFromEnd)
            {
                throw new NotSupportedException("Range must be closed");
            }
            
            return new IntEnumerator(range.Start.Value, range.End.Value);
        }

        public static IntEnumerator GetEnumerator(this int count)
        {
            return new IntEnumerator(0, count);
        }

        public ref struct IntEnumerator
        {
            private readonly int _end;
            private readonly int _step;
            
            public IntEnumerator(int start, int end)
            {
                _step = start <= end ? 1 : -1;
                Current = start - _step;
                _end = end;
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