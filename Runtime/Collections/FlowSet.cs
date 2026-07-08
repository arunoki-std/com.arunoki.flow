using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Collections
{
    /// Ordered unique collection.
    /// Iteration order: insertion order (oldest to newest).
    /// Internal storage may differ to allow removing current element during iteration.
    // Kept over HashSet<T>/List<T> (RF-008 audit): callers rely on safe removal of the current
    //   element while reverse-iterating (EventBus.UnregisterSource, Channel dispatch/Clear,
    //   HubContainer) AND on the Container<> root-propagation callbacks (OnElementAdded/Removed) —
    //   neither is offered by the BCL. Insertion order + indexed access are also part of the contract.
    // Contract: single-thread only (Unity main thread). Elements is mutated without synchronization;
    //   add a lock or concurrent variant only if off-thread access is ever introduced.
    public partial class FlowSet<TElement> : Container<TElement>
    {
        private readonly Func<TElement, bool> consumablePredicate;

        /// Iteration order: insertion order (oldest to newest)
        protected readonly List<TElement> Elements = new(16);

        public FlowSet(Func<TElement, bool> consumablePredicate = null)
            : base(null)
        {
            this.consumablePredicate = consumablePredicate;
        }

        public FlowSet(
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
