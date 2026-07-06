using System.Collections.Generic;

namespace Arunoki.Collections.Enumerators
{
    /// Returns reversed enumerator
    public readonly struct MutablePairWithIndex<TKey, TValue>
    {
        private readonly List<Pair<TKey, TValue>> list;

        public MutablePairWithIndex(List<Pair<TKey, TValue>> list)
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

            public (int index, TKey key, TValue element) Current
            {
                get
                {
                    var pair = list[index];
                    return (index, pair.Key, pair.Element);
                }
            }
        }
    }
}
