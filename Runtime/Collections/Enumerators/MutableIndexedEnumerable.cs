using System.Collections.Generic;

namespace Arunoki.Collections.Enumerators
{
    /// Returns reversed enumerator
    public readonly struct MutableIndexedEnumerable<T>
    {
        private readonly List<T> list;

        public MutableIndexedEnumerable(List<T> list)
        {
            this.list = list;
        }

        public Enumerator GetEnumerator() => new(list);

        /// Reversed enumerator
        public struct Enumerator
        {
            private readonly List<T> list;
            private int index;

            public Enumerator(List<T> list)
            {
                this.list = list;
                index = list.Count;
            }

            public bool MoveNext() => --index > -1;

            public (int index, T value) Current => (index, list[index]);
        }
    }
}
