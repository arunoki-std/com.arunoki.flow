using System;
using System.Collections.Generic;

namespace Arunoki.Collections.Enumerators
{
    /// Returns reversed enumerator
    public readonly struct MutableWhereEnumerable<T>
    {
        private readonly List<T> list;
        private readonly Func<T, bool> predicate;

        public MutableWhereEnumerable(List<T> list, Func<T, bool> predicate)
        {
            this.list = list;
            this.predicate = predicate;
        }

        public Enumerator GetEnumerator() => new(list, predicate);

        /// Reversed enumerator
        public struct Enumerator
        {
            private readonly List<T> list;
            private readonly Func<T, bool> predicate;
            private int index;
            private T current;

            public Enumerator(List<T> list, Func<T, bool> predicate)
            {
                this.list = list;
                this.predicate = predicate;
                index = list.Count;
                current = default;
            }

            public T Current => current;

            public bool MoveNext()
            {
                while (--index > -1)
                {
                    T item = list[index];
                    if (predicate(item))
                    {
                        current = item;
                        return true;
                    }
                }

                return false;
            }

            public void Reset()
            {
                index = list.Count;
                current = default;
            }

            public void Dispose() { }
        }
    }
}
