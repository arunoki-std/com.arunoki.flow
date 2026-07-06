namespace Arunoki.Collections
{
    public abstract class Container<TElement> : IContainer<TElement>
    {
        private readonly IContainer<TElement> rootContainer;

        protected Container(IContainer<TElement> rootContainer)
        {
            this.rootContainer = rootContainer;
        }

        void IContainer<TElement>.OnAdded(TElement element) => OnElementAdded(element);

        void IContainer<TElement>.OnRemoved(TElement element) => OnElementRemoved(element);

        protected virtual void OnElementAdded(TElement element)
        {
            rootContainer?.OnAdded(element);
        }

        protected virtual void OnElementRemoved(TElement element)
        {
            rootContainer?.OnRemoved(element);
        }
    }
}
