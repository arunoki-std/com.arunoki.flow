using System.Collections;
using System.Collections.Generic;

namespace Arunoki.Collections.Enumerators
{
    /// Enumerator is reversed
    // TODO [RF-008] This (and the other Mutable* enumerators) exists to allow removing the
    //   current element mid-iteration — the reason BCL enumerators were not used. Re-evaluate:
    //   reverse for-loops at call sites may make the whole Enumerators/ folder unnecessary.
    // TODO [RF-008] NOT thread-safe and unversioned: concurrent mutation of the underlying
    //   list corrupts iteration silently.
    public struct MutableEnumerator<T> : IEnumerator<T>
    {
        private readonly List<T> list;
        private int index;

        public MutableEnumerator(List<T> list)
        {
            this.list = list;
            index = list.Count;
        }

        public T Current => list[index];
        object IEnumerator.Current => Current!;

        public bool MoveNext() => --index > -1;

        public void Reset() => index = list.Count;

        public void Dispose() { }
    }
}
