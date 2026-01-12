using Arunoki.Collections;
using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Misc
{
  public abstract class CollectionService<TElement> : Container<TElement>, ISet<TElement>, IContextPart, IHubPart,
    IBuilder, IService
  {
    private bool isInitialized;

    protected CollectionService (IContainer<TElement> targetContainer = null) : base (targetContainer)
    {
    }

    public FlowHub Hub { get; private set; }
    public IContext Context { get; private set; }

    /// Concrete set.
    protected abstract ISet<TElement> GetSet ();

    IContext IContextPart.Get () => Context;

    void IContextPart.Set (IContext value)
    {
      if (Utils.IsDebug () && (Context != null && value != null))
        throw new InvalidOperationException ($"Trying to rewrite existing {nameof(Context)} '{Context}' by '{value}'.");

      Context = value;
    }

    FlowHub IHubPart.Get () => Hub;

    void IHubPart.Set (FlowHub value)
    {
      if (Utils.IsDebug () && (Hub != null && value != null))
        throw new InvalidOperationException ($"Trying to rewrite existing {nameof(Hub)} '{Hub}' by '{value}'.");

      Hub = value;
    }

    void IBuilder.Produce (object element) => Produce (element);
    protected abstract void Produce (object element);
    public abstract bool IsConsumable (object element);

    public int Count => GetSet ().Count;

    public void RemoveWhere (Func<TElement, bool> condition)
      => GetSet ().RemoveWhere (condition);

    public void ForEach (Action<TElement> action)
      => GetSet ().ForEach (action);

    public void Cast<T> (Action<T> action)
      => GetSet ().Cast (action);

    public void Cast<T> (Func<T, bool> condition, Action<T> action)
      => GetSet ().Cast (condition, action);

    public void Where (Func<TElement, bool> condition, Action<TElement> action)
      => GetSet ().Where (condition, action);

    public bool Any (Func<TElement, bool> condition)
      => GetSet ().Any (condition);

    /// Clear all elements.
    public virtual void Clear ()
    {
      GetSet ().Clear ();
    }

    /// To override.
    protected virtual void OnInitialized () { }

    /// To override.
    protected virtual void OnActivate () { }

    /// To override.
    protected virtual void OnDeactivate () { }

    public bool IsActive { get; private set; }

    protected internal void Initialize ()
    {
      if (!isInitialized)
      {
        isInitialized = true;
        OnInitialized ();
      }
    }

    public void Activate ()
    {
      Initialize ();

      if (!IsActive)
      {
        IsActive = true;
        OnActivate ();
      }
    }

    public void Deactivate ()
    {
      if (IsActive)
      {
        IsActive = false;
        OnDeactivate ();
      }
    }
  }
}