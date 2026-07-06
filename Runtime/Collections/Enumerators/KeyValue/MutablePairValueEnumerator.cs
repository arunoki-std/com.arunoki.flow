using System.Collections;
using System.Collections.Generic;

namespace Arunoki.Collections.Enumerators
{
    /// Reversed enumerator
    public struct MutablePairValueEnumerator<TKey, TValue> : IEnumerator<TValue>
    {
        private readonly List<Pair<TKey, TValue>> list;
        private int index;

        public MutablePairValueEnumerator(List<Pair<TKey, TValue>> list)
        {
            this.list = list;
            index = list.Count;
        }

        public bool MoveNext() => --index > -1;

        public void Reset()
        {
            index = list.Count;
        }

        object IEnumerator.Current => Current!;

        public TValue Current => list[index].Element;

        public void Dispose() { }
    }
}
