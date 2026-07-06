using System;
using System.Collections.Generic;

namespace Arunoki.Collections
{
    /// Ordered unique collection.
    /// Iteration order: insertion order (oldest to newest).
    /// Internal storage may differ to allow removing current element during iteration.
    // TODO [RF-008] Custom collection duplicating BCL functionality. Evaluate replacing with
    //   HashSet<T> / List<T> combo (note: List-backed Contains is O(n) here). The one feature
    //   BCL lacks is safe removal of the current element during iteration — keep only if that
    //   capability is proven necessary.
    // TODO [RF-008] NOT thread-safe: unsynchronized List<T> state. Add synchronization or a
    //   concurrent variant before any multithreaded use.
    public partial class Set<TElement> : Container<TElement>
    {
        private readonly Func<TElement, bool> consumablePredicate;

        /// Iteration order: insertion order (oldest to newest)
        protected readonly List<TElement> Elements = new(16);

        public Set(Func<TElement, bool> consumablePredicate = null)
            : base(null)
        {
            this.consumablePredicate = consumablePredicate;
        }

        public Set(
            IContainer<TElement> rootContainer,
            Func<TElement, bool> consumablePredicate = null
        )
            : base(rootContainer)
        {
            this.consumablePredicate = consumablePredicate;
        }

        public TElement this[int index] => Elements[(Elements.Count - 1) - index];
        public int Count => Elements.Count;
        internal bool IsEmpty => Elements.Count == 0;

        public bool Contains(TElement element) => Elements.Contains(element);

        public virtual bool TryAdd(TElement element)
        {
            if (!IsConsumable(element))
                return false;

            if (Elements.Contains(element))
                return false;

            Elements.Insert(0, element);
            OnElementAdded(element);

            return true;
        }

        public virtual void AddRange(params TElement[] elements)
        {
            for (var i = 0; i < elements.Length; i++)
                TryAdd(elements[i]);
        }

        public virtual bool Remove(TElement element)
        {
            return RemoveAt(Elements.IndexOf(element));
        }

        public virtual bool RemoveAt(int index)
        {
            if (index > -1 && index < Elements.Count)
            {
                var element = Elements[index];
                Elements.RemoveAt(index);
                OnElementRemoved(element);
                return true;
            }

            return false;
        }

        public virtual bool IsConsumable(TElement element) =>
            consumablePredicate?.Invoke(element) ?? element is not null;

        /// Iteration order: insertion order (oldest to newest)
        public List<TElement> GetList() => Elements;
    }
}
