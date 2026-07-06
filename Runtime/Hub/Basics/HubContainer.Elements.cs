using System;
using System.Collections.Generic;
using Arunoki.Collections;
using Arunoki.Collections.Enumerators;

namespace Arunoki.Flow.Basics
{
    public abstract partial class HubContainer<TElement>
    {
        private readonly List<TElement> all = new(32);

        protected internal Set<TElement> Set { get; }

        protected internal SetsTypeCollection<TElement> KeySet { get; }

        protected virtual void OnElementAdded(TElement element)
        {
            if (!IsMultiInstancesSupported())
            {
                var type = element as Type ?? element.GetType();
                if (!cachedTypes.Contains(type))
                    cachedTypes.Add(type);
                else
                    throw BuildOperationException.MultiInstancesNotSupported(element, this);
            }

            all.Add(element);
            Hub.TryInjectDependencies(element);

            if (
                IsInitialized()
                && element is IInitializable initializable
                && !initializable.IsInitialized()
            )
                initializable.Initialize();

            if (
                IsStarted()
                && element is IStartable startable
                && !startable.IsStarted()
                && element is not IManualService
            )
                startable.Start();
        }

        protected virtual void OnElementRemoved(TElement element)
        {
            if (!IsMultiInstancesSupported())
                cachedTypes.Remove(element.GetType());

            all.Remove(element);
        }

        /// To override.
        protected virtual void OnKeyAdded(Type key) { }

        /// To override.
        protected virtual void OnKeyRemoved(Type key) { }

        protected internal List<TElement> GetAllElements() => all;

        public MutableEnumerator<TElement> GetEnumerator() => new(all);

        public MutableCastEnumerable<TElement, T> Cast<T>() => new(all);

        private class KeyContainer : IContainer<Type>
        {
            private readonly HubContainer<TElement> container;

            public KeyContainer(HubContainer<TElement> container) => this.container = container;

            public void OnAdded(Type key) => container.OnKeyAdded(key);

            public void OnRemoved(Type key) => container.OnKeyRemoved(key);
        }

        private class Container : IContainer<TElement>
        {
            private readonly HubContainer<TElement> container;

            public Container(HubContainer<TElement> container) => this.container = container;

            public void OnAdded(TElement element) => container.OnElementAdded(element);

            public void OnRemoved(TElement element) => container.OnElementRemoved(element);
        }
    }
}
