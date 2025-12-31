using Arunoki.Collections;

namespace Arunoki.Flow.Misc
{
  public abstract class ReceiversGroup : EventsHubGroup<IEventsHandler>, IBuilder
  {
    protected override void OnElementAdded (IEventsHandler element)
    {
      base.OnElementAdded (element);

      Hub.Events.Subscribe (element);
    }

    protected override void OnElementRemoved (IEventsHandler element)
    {
      base.OnElementRemoved (element);

      Hub.Events.Unsubscribe (element);
    }

    protected abstract void Produce (object item);

    protected abstract bool IsConsumable (object item);

    void IBuilder.Build (object item) => Produce (item);

    bool IBuilder.IsConsumable (object item) => IsConsumable (item);
  }
}