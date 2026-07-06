using System;
using System.Collections.Generic;
using System.Linq;
using Arunoki.Collections.Enumerators;

namespace Arunoki.Collections
{
    // TODO [RF-008] NOT thread-safe: SetsCache/SetsList mutated without synchronization.
    //   If multithreaded access is needed, guard with a lock or move to ConcurrentDictionary.
    public class SetsTypeCollection<TElement> : Container<TElement>, ISet<TElement>
    {
        private readonly Func<TElement, bool> consumablePredicate;

        protected readonly Dictionary<Type, Set<TElement>> SetsCache = new(16);
        protected readonly List<Set<TElement>> SetsList = new(16);

        private readonly IContainer<Type> rootKeyContainer;

        public SetsTypeCollection(Func<TElement, bool> consumablePredicate = null)
            : this(null, null, consumablePredicate) { }

        public SetsTypeCollection(
            IContainer<Type> rootKeyContainer,
            Func<TElement, bool> consumablePredicate = null
        )
            : this(null, rootKeyContainer, consumablePredicate) { }

        public SetsTypeCollection(
            IContainer<TElement> rootElementsContainer,
            IContainer<Type> rootKeyContainer = null,
            Func<TElement, bool> consumablePredicate = null
        )
            : base(rootElementsContainer)
        {
            this.consumablePredicate = consumablePredicate;
            this.rootKeyContainer = rootKeyContainer;
        }

        protected virtual void OnKeyAdded(Type keyType)
        {
            rootKeyContainer?.OnAdded(keyType);
        }

        protected virtual void OnKeyRemoved(Type keyType)
        {
            rootKeyContainer?.OnRemoved(keyType);
        }

        public int Count
        {
            get
            {
                var count = 0;
                for (var i = 0; i < SetsList.Count; i++)
                    count += SetsList[i].Count;
                return count;
            }
        }

        public void TryAddRange(Type keyType, params TElement[] elements)
        {
            var set = GetOrCreate(keyType);

            for (var i = 0; i < elements.Length; i++)
                set.TryAdd(elements[i]);
        }

        public bool TryAdd(Type keyType, TElement element)
        {
            return GetOrCreate(keyType).TryAdd(element);
        }

        public void RemoveWhere(Func<TElement, bool> condition)
        {
            for (int index = 0; index < SetsList.Count; index++)
                SetsList[index].RemoveWhere(condition);
        }

        public bool Remove(TElement element)
        {
            for (int index = 0; index < SetsList.Count; index++)
                if (SetsList[index].Remove(element))
                    return true;

            return false;
        }

        public void ForEach(Predicate<TElement> condition, Action<TElement> action)
        {
            for (var i = 0; i < SetsList.Count; i++)
            {
                var set = SetsList[i];

                for (var index = 0; index < set.Count; index++)
                    if (condition(set[index]))
                        action(set[index]);
            }
        }

        public void Cast<T>(Action<T> action)
        {
            for (var i = 0; i < SetsList.Count; i++)
            {
                var set = SetsList[i];

                for (var index = 0; index < set.Count; index++)
                    if (set[index] is T cast)
                        action(cast);
            }
        }

        public void Cast<T>(Func<T, bool> condition, Action<T> action)
        {
            for (var i = 0; i < SetsList.Count; i++)
            {
                var set = SetsList[i];

                for (var index = 0; index < set.Count; index++)
                    if (set[index] is T cast && condition(cast))
                        action(cast);
            }
        }

        public void ForEach(Action<TElement> action)
        {
            for (var i = 0; i < SetsList.Count; i++)
            {
                var set = SetsList[i];

                for (var index = 0; index < set.Count; index++)
                    action(set[index]);
            }
        }

        public void Where(Func<TElement, bool> condition, Action<TElement> action)
        {
            for (var i = 0; i < SetsList.Count; i++)
            {
                var set = SetsList[i];

                for (var index = 0; index < set.Count; index++)
                {
                    var element = set[index];
                    if (condition(element))
                        action(element);
                }
            }
        }

        public bool Any(Func<TElement, bool> condition)
        {
            for (var i = 0; i < SetsList.Count; i++)
                if (SetsList[i].Any(condition))
                    return true;
            return false;
        }

        public void Clear<T>() => Clear(typeof(T));

        public virtual void Clear(Type keyType)
        {
            if (SetsCache.TryGetValue(keyType, out Set<TElement> set))
            {
                set.Clear();

                SetsCache.Remove(keyType);
                OnKeyRemoved(keyType);
            }
        }

        public virtual void Clear()
        {
            foreach (var key in SetsCache.Keys.ToArray())
                Clear(key);
        }

        public bool Contains(TElement item)
        {
            for (var i = 0; i < SetsList.Count; i++)
                if (SetsList[i].Contains(item))
                    return true;

            return false;
        }

        public Set<TElement> GetOrCreate<TType>() => GetOrCreate(typeof(TType));

        public Set<TElement> GetOrCreate(Type type)
        {
            if (!SetsCache.TryGetValue(type, out Set<TElement> set))
            {
                set = new Set<TElement>(this, consumablePredicate);
                SetsList.Add(set);
                SetsCache.Add(type, set);
                OnKeyAdded(type);
            }

            return set;
        }

        public Set<TElement> Get<TType>() => SetsCache[typeof(TType)];

        public bool TryGet<TType>(out Set<TElement> set) =>
            SetsCache.TryGetValue(typeof(TType), out set);

        public SetListEnumerator<TElement> GetEnumerator() => new(SetsList);
    }
}
