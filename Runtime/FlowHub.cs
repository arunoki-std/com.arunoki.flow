using Arunoki.Collections;
using Arunoki.Flow.Collections;

using System;

namespace Arunoki.Flow
{
  public partial class FlowHub : SetsCollection<IHandler>
  {
    public FlowHub (IContext entityContext, bool autoInit = true)
    {
      EntityContext = entityContext;
      Contexts = new ContextsCollection (this, EntityContext);

      if (EntityContext is IContainer<IHandler> c) SetTargetContainer (c);

      AddSetsFrom (this);
      AddSetsFrom (EntityContext);

      ForEachSet<IHubPart> (part => part.Set (this));
      ForEachSet<IContextPart> (part => part.Set (EntityContext));

      if (autoInit) Initialize ();
    }

    protected IContext EntityContext { get; }

    public EventBus Events { get; } = new();

    public ContextsCollection Contexts { get; }

    public PipelineSet Pipeline { get; } = new();

    public HandlerSet Handlers { get; } = new();

    protected override void OnElementAdded (IHandler element)
    {
      base.OnElementAdded (element);

      if (element is IHubPart hubPart && hubPart.Get () == null) hubPart.Set (this);
      if (element is IContextPart ctxPart && ctxPart.Get () == null) ctxPart.Set (EntityContext);
    }

    protected override void OnElementRemoved (IHandler element)
    {
      base.OnElementRemoved (element);

      if (element is IDisposable disposable) disposable.Dispose ();
      if (element is IContextPart ctxPart) ctxPart.Set (null);
      if (element is IHubPart hubPart) hubPart.Set (null);
    }

    /// Dispose all elements.
    public override void Clear ()
    {
      base.Clear ();

      Events.Clear ();
      Contexts.Clear ();
    }
  }
}