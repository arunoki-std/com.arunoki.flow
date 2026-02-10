using Arunoki.Collections;
using Arunoki.Collections.Enumerators;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Basics
{
  public abstract partial class HubBuilder<TElement> : IContainer<TElement>, IContainer<Type>
  {
    private readonly List<TElement> all = new(32);

    protected internal Set<TElement> Set { get; }

    protected internal SetsTypeCollection<TElement> KeySet { get; }

    protected virtual void OnElementAdded (TElement element)
    {
      if (Utils.IsDebug () && !IsMultiInstancesSupported ())
      {
        var type = element as Type ?? element.GetType ();
        if (!cachedTypes.Contains (type)) cachedTypes.Add (type);
        else throw BuildOperationException.MultiInstancesNotSupported (element, this);
      }

      (this as IContainer<TElement>).RootContainer?.OnAdded (element);
      all.Add (element);
    }

    protected virtual void OnElementRemoved (TElement element)
    {
      (this as IContainer<TElement>).RootContainer?.OnRemoved (element);

      if (Utils.IsDebug () && !IsMultiInstancesSupported ())
        cachedTypes.Remove (element.GetType ());

      all.Remove (element);
    }

    public void Clear (TElement element)
    {
      if (element == null) throw new ArgumentNullException (nameof(element));

      if (!Set.Remove (element))
        KeySet.Remove (element);
    }

    public virtual void ClearAll ()
    {
      Set.Clear ();
      KeySet.Clear ();
    }

    protected virtual void OnKeyAdded (Type key)
    {
      (this as IContainer<Type>).RootContainer?.OnAdded (key);
    }

    protected virtual void OnKeyRemoved (Type key)
    {
      (this as IContainer<Type>).RootContainer?.OnRemoved (key);
    }

    IContainer<TElement> IContainer<TElement>.RootContainer { get; set; }
    void IContainer<TElement>.OnAdded (TElement element) => OnElementAdded (element);
    void IContainer<TElement>.OnRemoved (TElement element) => OnElementRemoved (element);

    void IContainer<Type>.OnAdded (Type key) => OnKeyAdded (key);
    void IContainer<Type>.OnRemoved (Type key) => OnKeyRemoved (key);
    IContainer<Type> IContainer<Type>.RootContainer { get; set; }

    public MutableEnumerator<TElement> GetEnumerator () => new(all);
    public MutableCastEnumerable<TElement, T> Cast<T> () => new(all);
  }
}