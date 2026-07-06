using System;

namespace Arunoki.Collections
{
    public partial class SetsCollection<TElement> : ISet<TElement>
    {
        public int Count
        {
            get
            {
                var count = 0;
                for (var index = 0; index < Sets.Count; index++)
                    count += Sets[index].Count;
                return count;
            }
        }

        public void ForEachSet<TSet>(Action<TSet> action)
        {
            for (var index = 0; index < Sets.Count; index++)
                if (Sets[index] is TSet set)
                    action(set);
        }

        public bool ForAnySet<TSet>(Func<TSet, bool> condition)
        {
            for (var index = 0; index < Sets.Count; index++)
                if (Sets[index] is TSet set && condition(set))
                    return true;

            return false;
        }

        public void RemoveWhere(Func<TElement, bool> condition)
        {
            for (var index = 0; index < Sets.Count; index++)
                Sets[index].RemoveWhere(condition);
        }

        public bool Any(Func<TElement, bool> condition)
        {
            for (var i = 0; i < Sets.Count; i++)
                if (Sets[i].Any(condition))
                    return true;
            return false;
        }

        /// For each element do <param name="action"></param> if it passes <param name="condition"></param>
        public void Where(Func<TElement, bool> condition, Action<TElement> action)
        {
            for (var index = 0; index < Sets.Count; index++)
                Sets[index].Where(condition, action);
        }

        /// Cast elements at each set and for each element of type T do <param name="action"></param>
        public void Cast<T>(Action<T> action)
        {
            for (var index = 0; index < Sets.Count; index++)
                Sets[index].Cast(action);
        }

        /// Cast elements at each set and for each element of type T do <param name="action"></param> if it passes <param name="condition"></param>
        public void Cast<T>(Func<T, bool> condition, Action<T> action)
        {
            for (var index = 0; index < Sets.Count; index++)
                Sets[index].Cast(condition, action);
        }

        /// For each element at each set
        public void ForEach(Action<TElement> action)
        {
            for (var index = 0; index < Sets.Count; index++)
                Sets[index].ForEach(action);
        }

        /// Clear elements at each set
        public virtual void Clear()
        {
            for (var index = 0; index < Sets.Count; index++)
                Sets[index].Clear();
        }

        public bool ContainsSet(object other)
        {
            return other is ISet<TElement> set && Sets.Contains(set);
        }

        public bool Contains(TElement item)
        {
            for (var index = 0; index < Sets.Count; index++)
                if (Sets[index].Contains(item))
                    return true;

            return false;
        }
    }
}
