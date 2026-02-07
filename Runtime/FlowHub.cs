using Arunoki.Collections;
using Arunoki.Collections.Utilities;
using Arunoki.Flow.Collections;
using Arunoki.Flow.Events;

using System;

namespace Arunoki.Flow
{
  public partial class FlowHub : SetsCollection<IHandler>
  {
    protected System.Collections.Generic.List<IService> Services = new(16);

    protected IContext EntityContext { get; }

    public EventBus Events { get; } = new();

    public ContextsCollection Contexts { get; }

    /// Part of the hub sets <see cref="Arunoki.Collections.ISet{TElement}"/> where element is <see cref="IHandler"/>.
    public PipelineSet Pipeline { get; } = new();

    /// Part of the hub sets <see cref="Arunoki.Collections.ISet{TElement}"/> where element is <see cref="IHandler"/>.
    public HandlerSet Handlers { get; } = new();

    public FlowHub (IContext entityContext, bool autoInit = true)
    {
      EntityContext = entityContext;
      Contexts = new ContextsCollection (this, EntityContext);

      if (EntityContext is IContainer<IHandler> c) SetTargetContainer (c);

      OnInitSets ();
      OnInitServices ();

      ForEachSet<IHubPart> (part => part.Set (this));
      ForEachSet<IContextPart> (part => part.Set (EntityContext));

      if (autoInit) Initialize ();
    }

    protected virtual void OnInitSets ()
    {
      AddSetsFrom (this);
      AddSetsFrom (EntityContext);
    }

    protected virtual void OnInitServices ()
    {
      this.FindProperties<IService> (OnTryAddService);
    }

    protected internal virtual bool OnTryAddService (IService service)
    {
      if (service is IContext)
        return false;

      if (Services.Contains (service))
        return false;

      Services.Add (service);
      OnInjectDependencies (service);

      return true;
    }

    protected override void OnSetAdded (ISet<IHandler> set)
    {
      base.OnSetAdded (set);

      if (set is IService service) OnTryAddService (service);
    }

    protected override void OnElementAdded (IHandler element)
    {
      base.OnElementAdded (element);
      OnInjectDependencies (element);
    }

    protected override void OnElementRemoved (IHandler element)
    {
      base.OnElementRemoved (element);

      if (element is IDisposable disposable) disposable.Dispose ();
      if (element is IContextPart ctxPart) ctxPart.Set (null);
      if (element is IHubPart hubPart) hubPart.Set (null);
    }

    protected virtual void OnInjectDependencies (object target)
    {
      if (target is IHubPart hubPart && hubPart.Get () == null) hubPart.Set (this);
      if (target is IContextPart ctxPart && ctxPart.Get () == null) ctxPart.Set (EntityContext);
    }

    /// Remove all elements from hub components and collections.
    public override void Clear ()
    {
      Events.Clear ();
      Contexts.Clear ();

      base.Clear ();
    }

    /// Reset all mapped contexts and their event sources (properties, proxy data, triggers ets.) if they are
    /// <see cref="IResettable"/> and <see cref="IResettable.AutoReset"/> enabled. 
    public virtual void Reset ()
    {
      Events.Reset ();
      Contexts.Reset ();
    }
  }
}