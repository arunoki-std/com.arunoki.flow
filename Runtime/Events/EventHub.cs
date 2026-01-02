using Arunoki.Collections;
using Arunoki.Flow.Misc;

namespace Arunoki.Flow
{
  public partial class EventHub : SetsCollection<IEventsHandler>
  {
    public IEventsContext Context { get; private set; }

    public EventBus Events { get; } = new();

    public ControllersGroup Controller { get; } = new();

    public void Init (IEventsContext eventsContext)
    {
      Context = eventsContext;

      if (Context is IElementHandler<IEventsHandler> elementHandler)
        InitElementHandler (elementHandler);

      AddSetsFrom (this);
      AddSetsFrom (Context);

      ForEachSet<IElementHandler<IEventsHandler>> (handler => handler.TargetHandler = this);
      ForEachSet<IEventsHubPart> (part => part.Init (this));

      Build (Context);
    }

    protected override void OnElementAdded (IEventsHandler element)
    {
      base.OnElementAdded (element);

      if (element is IEventsContextPart contextPart) contextPart.Context = Context;
    }

    public override void Clear ()
    {
      base.Clear ();

      Events.Dispose ();
    }
  }
}