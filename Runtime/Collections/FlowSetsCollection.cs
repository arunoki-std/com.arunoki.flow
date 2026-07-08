using System;
using System.Collections.Generic;
using Arunoki.Flow.Collections.Enumerators;

namespace Arunoki.Flow.Collections
{
    // Contract: single-thread only (Unity main thread). SetsCache/SetsList are mutated without
    //   synchronization; flow's consumers (Hub) are main-thread. Guard with a lock or move to
    //   ConcurrentDictionary only if off-thread access is ever introduced.
    public class FlowSetsCollection<TElement> : Container<TElement>, IFlowSet<TElement>
    {
        private readonly Func<TElement, bool> consumablePredicate;

        protected readonly Dictionary<Type, FlowSet<TElement>> SetsCache = new(16);
        protected readonly List<FlowSet<TElement>> SetsList = new(16);

        private readonly IContainer<Type> rootKeyContainer;

        public FlowSetsCollection(Func<TElement, bool> consumablePredicate = null)
            : this(null, null, consumablePredicate) { }

        public FlowSetsCollection(
            IContainer<Type> rootKeyContainer,
            Func<TElement, bool> consumablePredicate = null
        )
            : this(null, rootKeyContainer, consumablePredicate) { }

        public FlowSetsCollection(
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
            if (SetsCache.TryGetValue(keyType, out FlowSet<TElement> set))
            {
                set.Clear();

                SetsCache.Remove(keyType);
                OnKeyRemoved(keyType);
            }
        }

        public virtual void Clear()
        {
            // Snapshot keys before iterating: Clear(key) mutates SetsCache. Plain CopyTo (BCL,
            // not System.Linq) replaces the former Keys.ToArray() per the no-LINQ convention.
            var keys = new Type[SetsCache.Count];
            SetsCache.Keys.CopyTo(keys, 0);

            for (var i = 0; i < keys.Length; i++)
                Clear(keys[i]);
        }

        public bool Contains(TElement item)
        {
            for (var i = 0; i < SetsList.Count; i++)
                if (SetsList[i].Contains(item))
                    return true;

            return false;
        }

        public FlowSet<TElement> GetOrCreate<TType>() => GetOrCreate(typeof(TType));

        public FlowSet<TElement> GetOrCreate(Type type)
        {
            if (!SetsCache.TryGetValue(type, out FlowSet<TElement> set))
            {
                set = new FlowSet<TElement>(this, consumablePredicate);
                SetsList.Add(set);
                SetsCache.Add(type, set);
                OnKeyAdded(type);
            }

            return set;
        }

        public FlowSet<TElement> Get<TType>() => SetsCache[typeof(TType)];

        public bool TryGet<TType>(out FlowSet<TElement> set) =>
            SetsCache.TryGetValue(typeof(TType), out set);

        public SetListEnumerator<TElement> GetEnumerator() => new(SetsList);
    }
}
