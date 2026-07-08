using System;

namespace Arunoki.Flow.Collections
{
    /// Query/iterate/mutate view over an ordered element set (implemented by <see cref="FlowSet{TElement}"/>
    /// and <see cref="FlowSetsCollection{TElement}"/>). Named to avoid colliding with
    /// <see cref="System.Collections.Generic.ISet{T}"/> (renamed from the former <c>ISet</c> in RF-008).
    public interface IFlowSet<TElement>
    {
        int Count { get; }

        void RemoveWhere(Func<TElement, bool> condition);

        void ForEach(Action<TElement> action);

        void Cast<T>(Action<T> action);

        void Cast<T>(Func<T, bool> condition, Action<T> action);

        void Where(Func<TElement, bool> condition, Action<TElement> action);

        bool Any(Func<TElement, bool> condition);

        void Clear();

        bool Contains(TElement item);
    }
}
