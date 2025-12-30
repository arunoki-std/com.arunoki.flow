using Arunoki.Flow;

using System;
using System.Collections.Generic;

namespace Arunoki.Collections
{
  public abstract class BaseGroup<TElement> : IGroup, IGroupHandler<TElement>, IDisposable
  {
    private IGroupHandler<TElement> groupHandler;
    private readonly Type elementType = typeof(TElement);

    protected BaseGroup ()
    {
    }

    protected BaseGroup (IGroupHandler<TElement> groupHandler)
    {
      this.groupHandler = groupHandler;
    }

    public void ForEach<T> (Action<T> action)
    {
      foreach (var element in GetAll<T> ())
        action (element);
    }

    public abstract IEnumerable<T> GetAll<T> ();

    public abstract IEnumerable<TElement> GetAll ();

    public abstract IEnumerable<T> Where<T> (Predicate<T> condition);

    public abstract IEnumerable<TElement> Where (Predicate<TElement> condition);

    public abstract void Select (Predicate<TElement> condition, Action<TElement> action);

    public abstract void Select<T> (Predicate<T> condition, Action<T> action);

    public abstract void ForEach (Action<TElement> action);

    public bool Is<T> () => typeof(T) == elementType;

    public abstract void Dispose ();

    IGroupHandler<TElement> IGroupHandler<TElement>.TargetGroupHandler
    {
      get => groupHandler;
      set => groupHandler = value;
    }

    void IGroupHandler<TElement>.OnAdded (TElement element) => OnAdded (element);

    void IGroupHandler<TElement>.OnRemoved (TElement element) => OnRemoved (element);

    protected virtual void OnAdded (TElement element)
    {
      groupHandler?.OnAdded (element);
    }

    protected virtual void OnRemoved (TElement element)
    {
      groupHandler?.OnRemoved (element);
    }
  }
}