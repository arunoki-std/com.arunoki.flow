using Arunoki.Collections;
using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  public partial class FlowHub : SetsCollection<IHandler>
  {
    public IContext Context { get; private set; }

    public EventBus Events { get; } = new();

    public PipelineSet Pipeline { get; } = new();

    public FlowHub (IContext context)
    {
      Context = context;

      if (Context is IContainer<IHandler> c) SetTargetContainer (c);

      AddSetsFrom (this);
      AddSetsFrom (Context);

      ForEachSet<IContextPart> (part => part.Set (Context));
      ForEachSet<IHubPart> (part => part.Set (this));

      Produce (Context);
    }

    protected override void OnElementAdded (IHandler element)
    {
      base.OnElementAdded (element);

      if (element is IContextPart ctxPart && ctxPart.Get () == null) ctxPart.Set (Context);
      if (element is IHubPart hubPart && hubPart.Get () == null) hubPart.Set (this);
    }

    protected override void OnElementRemoved (IHandler element)
    {
      base.OnElementRemoved (element);

      if (element is IDisposable disposable) disposable.Dispose ();
      if (element is IContextPart ctxPart) ctxPart.Set (null);
      if (element is IHubPart hubPart) hubPart.Set (null);
    }

    public override void Clear ()
    {
      base.Clear ();

      Events.Clear ();
    }
  }
}