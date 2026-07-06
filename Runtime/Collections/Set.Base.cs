using System;

namespace Arunoki.Collections
{
    public partial class Set<TElement> : ISet<TElement>
    {
        public bool Any(Func<TElement, bool> condition)
        {
            for (var index = Elements.Count - 1; index >= 0; index--)
                if (condition(Elements[index]))
                    return true;
            return false;
        }

        public void RemoveWhere(Func<TElement, bool> condition)
        {
            for (var index = Elements.Count - 1; index >= 0; index--)
                if (condition(Elements[index]))
                    RemoveAt(index);
        }

        public void ForEach(Action<TElement> action)
        {
            for (var index = Elements.Count - 1; index > -1 && index < Elements.Count; index--)
                action(Elements[index]);
        }

        public void Cast<T>(Action<T> action)
        {
            for (var index = Elements.Count - 1; index > -1; index--)
                if (Elements[index] is T cast)
                    action(cast);
        }

        public void Cast<T>(Func<T, bool> condition, Action<T> action)
        {
            for (var index = Elements.Count - 1; index > -1; index--)
                if (Elements[index] is T cast && condition(cast))
                    action(cast);
        }

        public void Where(Func<TElement, bool> condition, Action<TElement> action)
        {
            for (var index = Elements.Count - 1; index > -1; index--)
            {
                var element = Elements[index];

                if (condition(element))
                    action(element);
            }
        }

        public virtual void Clear()
        {
            for (var index = Elements.Count - 1; index >= 0; index--)
                RemoveAt(index);
        }
    }
}
