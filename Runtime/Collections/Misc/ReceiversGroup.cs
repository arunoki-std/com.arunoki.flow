using Arunoki.Collections;

using System;

namespace Arunoki.Flow.Misc
{
  public abstract class ReceiversGroup : EventsHubGroup<IEventReceiver>, IBuilder
  {
    protected override void OnAdded (IEventReceiver element)
    {
      base.OnAdded (element);

      Hub.Events.Subscribe (element);
    }

    protected override void OnRemoved (IEventReceiver element)
    {
      base.OnRemoved (element);

      Hub.Events.Unsubscribe (element);
    }

    protected abstract void Produce (object item);

    protected abstract bool IsConsumable (object item);

    protected abstract bool IsConsumable (Type itemType);

    void IBuilder.Build (object item) => Produce (item);

    bool IBuilder.IsConsumable (object item) => IsConsumable (item);

    bool IBuilder.IsConsumable (Type itemType) => IsConsumable (itemType);
  }
}