using Arunoki.Collections;

namespace Arunoki.Flow.Misc
{
  public class PureHandlers : HandlerSet
  {
    protected Set<IHandler> Handlers = new();

    protected override ISet<IHandler> GetConcreteSet () => Handlers;

    protected override void Produce (object element)
    {
      Handlers.Add (element as IHandler);
    }

    protected override void OnElementAdded (IHandler element)
    {
      base.OnElementAdded (element);

      Hub.Events.Subscribe (element);
    }

    protected override void OnElementRemoved (IHandler element)
    {
      base.OnElementRemoved (element);

      Hub.Events.Unsubscribe (element);
    }

    public override bool IsConsumable (object element)
      => element is IHandler;
  }
}