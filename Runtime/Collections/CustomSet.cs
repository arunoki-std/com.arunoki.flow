using System;

namespace Arunoki.Collections
{
    // TODO [RF-008] Thread-safety depends entirely on the concrete GetSet() implementation;
    //   no synchronization is provided here.
    public abstract class CustomSet<TElement> : Container<TElement>, ISet<TElement>
    {
        protected CustomSet(IContainer<TElement> rootContainer = null)
            : base(rootContainer) { }

        public int Count => GetSet().Count;

        /// Concrete set.
        protected abstract ISet<TElement> GetSet();

        public bool Contains(TElement item) => GetSet().Contains(item);

        public void RemoveWhere(Func<TElement, bool> condition) => GetSet().RemoveWhere(condition);

        public void ForEach(Action<TElement> action) => GetSet().ForEach(action);

        public void Cast<T>(Action<T> action) => GetSet().Cast(action);

        public void Cast<T>(Func<T, bool> condition, Action<T> action) =>
            GetSet().Cast(condition, action);

        public void Where(Func<TElement, bool> condition, Action<TElement> action) =>
            GetSet().Where(condition, action);

        public bool Any(Func<TElement, bool> condition) => GetSet().Any(condition);

        /// Clear all elements.
        public virtual void Clear()
        {
            GetSet().Clear();
        }
    }
}
