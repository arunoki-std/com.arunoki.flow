using Arunoki.Collections;
using Arunoki.Collections.Enumerators;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Basics
{
  public abstract partial class HubBuilder<TElement>
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

      all.Add (element);
    }

    protected virtual void OnElementRemoved (TElement element)
    {
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

    /// To override.
    protected virtual void OnKeyAdded (Type key) { }

    /// To override.
    protected virtual void OnKeyRemoved (Type key) { }

    public MutableEnumerator<TElement> GetEnumerator () => new(all);
    public MutableCastEnumerable<TElement, T> Cast<T> () => new(all);

    private class KeyContainer : IContainer<Type>
    {
      private readonly HubBuilder<TElement> builder;
      public KeyContainer (HubBuilder<TElement> builder) => this.builder = builder;
      public void OnAdded (Type key) => builder.OnKeyAdded (key);
      public void OnRemoved (Type key) => builder.OnKeyRemoved (key);
    }

    private class Container : IContainer<TElement>
    {
      private readonly HubBuilder<TElement> builder;
      public Container (HubBuilder<TElement> builder) => this.builder = builder;
      public void OnAdded (TElement element) => builder.OnElementAdded (element);
      public void OnRemoved (TElement element) => builder.OnElementRemoved (element);
    }
  }
}