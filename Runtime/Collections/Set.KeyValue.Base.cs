using System;

namespace Arunoki.Collections
{
    public partial class Set<TKey, TElement>
    {
        public void RemoveWhere(Func<TElement, bool> condition)
        {
            for (var index = Elements.Count - 1; index >= 0; index--)
                if (condition(Elements[index].Element))
                    RemoveAt(index);
        }

        public void ForEach(Action<TElement> action)
        {
            for (var i = Elements.Count - 1; i > -1 && i < Elements.Count; i--)
                action(Elements[i].Element);
        }

        public void Cast<T>(Action<T> action)
        {
            for (var index = Elements.Count - 1; index > -1; index--)
                if (Elements[index].Element is T cast)
                    action(cast);
        }

        public void Cast<T>(Func<T, bool> condition, Action<T> action)
        {
            for (var index = Elements.Count - 1; index > -1; index--)
                if (Elements[index].Element is T cast && condition(cast))
                    action(cast);
        }

        public void Where(Func<TElement, bool> condition, Action<TElement> action)
        {
            for (var index = Elements.Count - 1; index > -1; index--)
            {
                var element = Elements[index].Element;

                if (condition(element))
                    action(element);
            }
        }

        public void Clear()
        {
            for (var index = Elements.Count - 1; index >= 0; index--)
                RemoveAt(index);

            Elements = new();
        }
    }
}
