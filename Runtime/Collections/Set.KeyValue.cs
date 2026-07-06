using System.Collections.Generic;

namespace Arunoki.Collections
{
    // TODO [RF-008] Duplicates Dictionary<TKey, TValue> + insertion order. Evaluate replacing
    //   with plain Dictionary<,> (or Unity.Collections native containers in job/Burst contexts).
    // TODO [RF-008] NOT thread-safe: List + Dictionary mutated without synchronization.
    //   Consider ConcurrentDictionary<,> if multithreaded access is ever needed.
    public partial class Set<TKey, TElement> : Container<TElement>
    {
        /// Reversed list.
        protected List<Pair<TKey, TElement>> Elements = new();

        protected Dictionary<TKey, TElement> ElementsByKey = new();

        public Set()
            : base(null) { }

        public Set(IContainer<TElement> rootContainer)
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
