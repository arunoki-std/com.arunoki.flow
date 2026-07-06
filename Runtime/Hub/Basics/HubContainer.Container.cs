using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Basics
{
    public abstract partial class HubContainer<TElement> : IHubContainer
        where TElement : class
    {
        private readonly List<Type> cachedTypes = new(16);

        bool IHubContainer.Register(object element) => Register(element as TElement);

        /// <summary>
        ///
        /// </summary>
        /// <param name="element"></param>
        /// <exception cref="ArgumentNullException"> <see cref="element"/> is null.</exception>
        /// <exception cref="BuildOperationException"><see cref="CanBuildAfterHubInit"/>, <see cref="CanBuildAfterHubStarted"/>, <see cref="CanBuildAfterHubActivation"/></exception>
        public virtual bool Register(TElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!CanBuildAfterHubInit() && Hub.IsInitialized())
                throw BuildOperationException.AfterHubInit(element);

            if (!CanBuildAfterHubStarted() && (Hub as IStartable).IsStarted())
                throw BuildOperationException.AfterHubStarted(element);

            if (!CanBuildAfterHubActivation() && Hub.IsActive())
                throw BuildOperationException.AfterHubActivated(element);

            return !GetAllElements().Contains(element) && Set.TryAdd(element);
        }

        void IHubContainer.Remove(object element) => Remove(element as TElement);

        public void Remove(TElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!Set.Remove(element))
                KeySet.Remove(element);
        }

        public virtual void RemoveAll()
        {
            Set.Clear();
            KeySet.Clear();
        }

        bool IHubContainer.IsConsumable(object element) => IsConsumable(element as TElement);

        public virtual bool IsConsumable(TElement element) =>
            element != null && element is not IDummy;

        /// Check whether is element type unique at elements collection. For debug build.
        protected virtual bool IsMultiInstancesSupported() => true;

        protected virtual bool CanBuildAfterHubInit() => true;

        protected virtual bool CanBuildAfterHubStarted() => true;

        protected virtual bool CanBuildAfterHubActivation() => true;

        public virtual int GetBuildOrder() => (int)FlowHub.BuildOrder.Any;
    }
}
