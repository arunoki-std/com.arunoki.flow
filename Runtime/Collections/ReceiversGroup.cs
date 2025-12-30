using Arunoki.Collections;

namespace Arunoki.Flow.Misc
{
  public abstract class ReceiversGroup : EventsHubGroup<IEventReceiver>, IBuilder
  {
    protected override void OnElementAdded (IEventReceiver element)
    {
      base.OnElementAdded (element);

      Hub.Events.Subscribe (element);
    }

    protected override void OnElementRemoved (IEventReceiver element)
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