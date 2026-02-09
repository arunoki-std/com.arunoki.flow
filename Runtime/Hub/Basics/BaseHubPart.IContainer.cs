using Arunoki.Collections;
using Arunoki.Collections.Enumerators;

namespace Arunoki.Flow.Basics
{
  public abstract partial class BaseHubPart<TElement> : IContainer<TElement> where TElement : class
  {
    private IContainer<TElement> rootContainer;

    protected BaseHubPart (IContainer<TElement> rootContainer = null)
    {
      this.rootContainer = rootContainer;
    }

    IContainer<TElement> IContainer<TElement>.RootContainer { get => rootContainer; set => rootContainer = value; }

    void IContainer<TElement>.OnAdded (TElement element) => OnElementAdded (element);
    void IContainer<TElement>.OnRemoved (TElement element) => OnElementRemoved (element);

    protected virtual void OnElementAdded (TElement element)
    {
      rootContainer?.OnAdded (element);
    }

    protected virtual void OnElementRemoved (TElement element)
    {
      rootContainer?.OnRemoved (element);
    }

    public abstract MutableEnumerator<TElement> GetEnumerator ();

    public abstract MutableCastEnumerable<TElement, T> Cast<T> ();
  }
}