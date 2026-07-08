using System.Collections;
using System.Collections.Generic;

namespace Arunoki.Flow.Collections.Enumerators
{
    /// Enumerator is reversed
    // Reverse iteration is intentional (RF-008 audit): it lets callers remove the current element
    //   mid-iteration — relied upon by EventBus.UnregisterSource, Channel dispatch/Clear and
    //   HubContainer — which BCL enumerators forbid. This is why the Mutable* enumerators exist.
    // Contract: single-thread only (Unity main thread). Unversioned: concurrent mutation of the
    //   underlying list corrupts iteration silently, so all mutation must stay on the main thread.
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
