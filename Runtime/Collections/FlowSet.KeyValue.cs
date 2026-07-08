using System.Collections.Generic;

namespace Arunoki.Flow.Collections
{
    // Kept over plain Dictionary<TKey, TValue> (RF-008 audit): the paired List preserves insertion
    //   order and enables safe removal of the current element while reverse-iterating (see the
    //   Mutable* enumerators / EventBus.Channels), plus Container<> root-propagation callbacks —
    //   a bare Dictionary offers none of these.
    // Contract: single-thread only (Unity main thread). List + Dictionary are mutated without
    //   synchronization; add a concurrent variant only if off-thread access is ever introduced.
    public partial class FlowSet<TKey, TElement> : Container<TElement>
    {
        /// Reversed list.
        protected List<Pair<TKey, TElement>> Elements = new();

        protected Dictionary<TKey, TElement> ElementsByKey = new();

        public FlowSet()
            : base(null) { }

        public FlowSet(IContainer<TElement> rootContainer)
            : base(rootContainer) { }

        public TElement this[TKey key] => ElementsByKey[key];

        public bool Contains(TKey key) => ElementsByKey.ContainsKey(key);

        public bool Contains(TElement element) => ElementsByKey.ContainsValue(element);

        public bool TryGet(TKey key, out TElement element) =>
            ElementsByKey.TryGetValue(key, out element);

        public virtual void Add(TKey key, TElement element)
        {
            Elements.Insert(0, new Pair<TKey, TElement>(key, element));
            ElementsByKey.Add(key, element);

            OnElementAdded(element);
        }

        public bool RemoveAt(int index)
        {
            if (index > -1 && index < Elements.Count)
            {
                var pair = Elements[index];

                Elements.RemoveAt(index);
                ElementsByKey.Remove(pair.Key);

                OnElementRemoved(pair.Element);
                return true;
            }

            return false;
        }

        public bool Remove(TElement element)
        {
            var index = Elements.FindIndex(pair => element.Equals(pair.Element));
            if (index > -1)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool Remove(TKey key)
        {
            var index = Elements.FindIndex(pair => key.Equals(pair.Key));
            if (index > -1)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }
    }
}
