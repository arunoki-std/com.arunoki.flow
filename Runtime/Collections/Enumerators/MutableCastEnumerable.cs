using System.Collections.Generic;

namespace Arunoki.Collections.Enumerators
{
    /// Returns reversed enumerator
    public readonly struct MutableCastEnumerable<TElement, TCast>
    {
        private readonly List<TElement> list;

        public MutableCastEnumerable(List<TElement> list)
        {
            this.list = list;
        }

        public Enumerator GetEnumerator() => new(list);

        /// Reversed enumerator
        public struct Enumerator
        {
            private readonly List<TElement> list;
            private int index;
            private TCast current;

            public Enumerator(List<TElement> list)
            {
                this.list = list;
                index = list.Count;
                current = default;
            }

            public TCast Current => current;

            public bool MoveNext()
            {
                while (--index > -1)
                {
                    if (list[index] is TCast cast)
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
