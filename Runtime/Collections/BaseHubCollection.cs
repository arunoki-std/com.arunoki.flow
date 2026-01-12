using Arunoki.Collections;
using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Misc
{
  public abstract class BaseHubCollection<TElement> : CustomSet<TElement>, IContextPart, IHubPart, IBuilder
  {
    private bool isInitialized;

    protected BaseHubCollection (IContainer<TElement> targetContainer = null) : base (targetContainer)
    {
    }

    public FlowHub Hub { get; private set; }
    public IContext Context { get; private set; }

    /// To override.
    protected virtual void OnInitialized () { }

    protected internal void Initialize ()
    {
      if (!isInitialized)
      {
        isInitialized = true;
        OnInitialized ();
      }
    }

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
  }
}