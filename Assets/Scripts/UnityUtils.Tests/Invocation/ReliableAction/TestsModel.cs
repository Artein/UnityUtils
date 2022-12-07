using System;

namespace Invocation.ReliableAction
{
    public class TestsModel
    {
        private int _count;

        public int Count
        {
            get => _count;
            set
            {
                if (_count != value)
                {
                    _count = value;
                    CountChanged?.Invoke(_count);
                }
            }
        }

        public event Action<int> CountChanged;
    }
}