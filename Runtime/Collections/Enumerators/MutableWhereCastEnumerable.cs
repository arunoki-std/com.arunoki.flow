using System;
using System.Collections.Generic;

namespace Arunoki.Collections.Enumerators
{
    /// Returns reversed enumerator
    public readonly struct MutableWhereCastEnumerable<TElement, TCast>
    {
        private readonly List<TElement> list;
        private readonly Func<TCast, bool> predicate;

        public MutableWhereCastEnumerable(List<TElement> list, Func<TCast, bool> predicate)
        {
            this.list = list;
            this.predicate = predicate;
        }

        public Enumerator GetEnumerator() => new(list, predicate);

        /// Reversed enumerator
        public struct Enumerator
        {
            private readonly List<TElement> list;
            private readonly Func<TCast, bool> predicate;
            private int index;
            private TCast current;

            public Enumerator(List<TElement> list, Func<TCast, bool> predicate)
            {
                this.list = list;
                this.predicate = predicate;
                index = list.Count;
                current = default;
            }

            public TCast Current => current;

            public bool MoveNext()
            {
                while (--index > -1)
                {
                    if (list[index] is TCast cast && predicate(cast))
                    {
                        current = cast;
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
