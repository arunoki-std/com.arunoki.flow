using Arunoki.Collections;
using Arunoki.Flow.Events.Core;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Events
{
  public partial class EventBus
  {
    public List<Callback> Subscribe (IHandler handler)
    {
      return EventBusUtility.Subscribe (this, handler);
    }

    public List<Callback> Subscribe (Type staticHandler)
    {
      return EventBusUtility.Subscribe (this, staticHandler);
    }

    public void Unsubscribe (Type staticHandler) => Unsubscribe (staticHandler as object);

    public void Unsubscribe (IHandler handler) => Unsubscribe (handler as object);

    protected virtual void Unsubscribe (object handler)
    {
      foreach (var channel in Channels)
        for (var i = channel.Callbacks.Count - 1; i >= 0; i--)
          if (channel.Callbacks [i].IsConsumable (handler))
            channel.RemoveAt (i);
    }

    public virtual void UnsubscribeAll ()
    {
      Channels.Clear ();
    }

    protected internal void Add (Channel channel)
    {
      Channels.Add (channel.GetEventType (), channel);
    }

    protected virtual void OnChannelAdded (Channel channel)
    {
    }

    protected virtual void OnChannelRemoved (Channel channel)
    {
      channel.Clear ();
    }

    private class Container : IContainer<Channel>
    {
      private readonly EventBus eventBus;
      public Container (EventBus eventBus) => this.eventBus = eventBus;
      public void OnAdded (Channel element) => eventBus.OnChannelAdded (element);
      public void OnRemoved (Channel element) => eventBus.OnChannelRemoved (element);
    }
  }
}