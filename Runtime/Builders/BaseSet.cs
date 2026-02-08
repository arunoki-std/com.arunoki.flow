using Arunoki.Collections;
using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Collections
{
  public class BaseSet<TEntity> : Set<TEntity>, IHubPart, IContextPart where TEntity : class
  {
    private bool isInitialized;

    protected BaseSet (IContainer<TEntity> rootContainer = null) : base (rootContainer)
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
        return;
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
  }
}