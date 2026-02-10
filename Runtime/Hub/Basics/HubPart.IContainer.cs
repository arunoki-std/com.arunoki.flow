using Arunoki.Collections;
using Arunoki.Collections.Enumerators;

namespace Arunoki.Flow.Basics
{
  public abstract partial class HubPart<TElement> : IContainer<TElement> where TElement : class
  {
    protected HubPart (IContainer<TElement> rootContainer = null)
    {
      (this as IContainer<TElement>).RootContainer = rootContainer;
    }

    IContainer<TElement> IContainer<TElement>.RootContainer { get; set; }

    void IContainer<TElement>.OnAdded (TElement element) => OnElementAdded (element);
    void IContainer<TElement>.OnRemoved (TElement element) => OnElementRemoved (element);

    protected virtual void OnElementAdded (TElement element)
    {
      (this as IContainer<TElement>).RootContainer?.OnAdded (element);
    }

    protected virtual void OnElementRemoved (TElement element)
    {
      (this as IContainer<TElement>).RootContainer?.OnRemoved (element);
    }

    public abstract MutableEnumerator<TElement> GetEnumerator ();

    public abstract MutableCastEnumerable<TElement, T> Cast<T> ();
  }
}