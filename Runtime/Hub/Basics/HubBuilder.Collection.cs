using Arunoki.Collections;
using Arunoki.Collections.Enumerators;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Basics
{
  public abstract partial class HubBuilder<TElement> : IContainer<Type>
  {
    private readonly List<TElement> allElements = new(32);

    protected internal Set<TElement> Elements { get; }

    protected internal SetsTypeCollection<TElement> KeySet { get; }

    protected HubBuilder (IContainer<TElement> rootContainer = null, IContainer<Type> rootKeyBuilder = null)
      : base (rootContainer)
    {
      (this as IContainer<Type>).RootContainer = rootKeyBuilder;

      Elements = new(this, IsConsumable);
      KeySet = new(this, this); //TODO: add isConsumable to set collection
    }

    protected override void OnElementAdded (TElement element)
    {
      if (Utils.IsDebug () && !IsMultiInstancesSupported ())
      {
        var type = element as Type ?? element.GetType ();
        if (!cachedTypes.Contains (type)) cachedTypes.Add (type);
        else throw BuildOperationException.MultiInstancesNotSupported (element, this);
      }

      base.OnElementAdded (element);
      allElements.Add (element);
    }

    protected override void OnElementRemoved (TElement element)
    {
      base.OnElementRemoved (element);

      if (Utils.IsDebug () && !IsMultiInstancesSupported ())
        cachedTypes.Remove (element.GetType ());

      allElements.Remove (element);
    }

    public void Clear (TElement element)
    {
      if (element == null) throw new ArgumentNullException (nameof(element));

      if (!Elements.Remove (element))
        KeySet.Remove (element);
    }

    public virtual void ClearAll ()
    {
      Elements.Clear ();
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

    void IContainer<Type>.OnAdded (Type key) => OnKeyAdded (key);
    void IContainer<Type>.OnRemoved (Type key) => OnKeyRemoved (key);
    IContainer<Type> IContainer<Type>.RootContainer { get; set; }

    public override MutableEnumerator<TElement> GetEnumerator () => new(allElements);

    public override MutableCastEnumerable<TElement, T> Cast<T> () => new(allElements);
  }
}