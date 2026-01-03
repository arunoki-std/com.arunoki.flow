using Arunoki.Collections;
using Arunoki.Flow.Misc;

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

      if (Context is IContainer<IHandler> elementHandler)
        SetTargetContainer (elementHandler);

      AddSetsFrom (this);
      AddSetsFrom (Context);

      ForEachSet<IContainer<IHandler>> (handler => handler.TargetContainer = this);
      ForEachSet<IHubPart> (part => part.Init (this));

      Build (Context);
    }

    protected override void OnElementAdded (IHandler element)
    {
      base.OnElementAdded (element);

      if (element is IContextPart contextPart) contextPart.Set (Context);
    }

    public override void Clear ()
    {
      base.Clear ();

      Events.Dispose ();
    }
  }
}