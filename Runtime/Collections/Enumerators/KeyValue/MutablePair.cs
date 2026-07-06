using System.Collections.Generic;

namespace Arunoki.Collections.Enumerators
{
    /// Returns reversed enumerator
    public readonly struct MutablePair<TKey, TValue>
    {
        private readonly List<Pair<TKey, TValue>> list;

        public MutablePair(List<Pair<TKey, TValue>> list)
        {
            this.list = list;
        }

        public Enumerator GetEnumerator() => new(list);

        /// Reversed enumerator
        public struct Enumerator
        {
            private readonly List<Pair<TKey, TValue>> list;
            private int index;

            public Enumerator(List<Pair<TKey, TValue>> list)
            {
                this.list = list;
                index = list.Count;
            }

            public bool MoveNext() => --index > -1;

            public (TKey key, TValue value) Current
            {
                get
                {
                    var pair = list[index];
                    return (pair.Key, pair.Element);
                }
            }
        }
    }
}
