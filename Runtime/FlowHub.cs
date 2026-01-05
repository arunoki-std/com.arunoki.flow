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

    public void Init (IContext context)
    {
      Context = context;

      if (Context is IContainer<IHandler> c) SetTargetContainer (c);

      AddSetsFrom (this);
      AddSetsFrom (Context);

      ForEachSet<IContextPart> (part => part.Set (Context));
      ForEachSet<IHubPart> (part => part.Init (this));

      Produce (Context);
    }

    protected override void OnElementAdded (IHandler element)
    {
      base.OnElementAdded (element);

      if (element is IContextPart part && part.Get () == null)
        part.Set (Context);

      if (element is IHubPart hubPart && hubPart.Hub == null) hubPart.Init (this);
    }

    protected override void OnElementRemoved (IHandler element)
    {
      base.OnElementRemoved (element);

      if (element is IDisposable disposable) disposable.Dispose ();
    }

    public override void Clear ()
    {
      base.Clear ();

      Events.Clear ();
    }
  }
}