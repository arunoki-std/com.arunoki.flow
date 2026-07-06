using System;

namespace Arunoki.Collections
{
    // TODO [RF-008] Name collides with System.Collections.Generic.ISet<T>; rename when the
    //   std-collections migration lands (renames are out of scope for RF-007).
    public interface ISet<TElement>
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
