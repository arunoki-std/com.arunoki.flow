using Arunoki.Collections;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Misc
{
  public class EventChannelSet
  {
    private readonly Dictionary<Type, EventChannel> typeSet = new();

    protected internal Set<EventChannel> Set = new();

    public EventChannel this [Type eventType]
    {
      get
      {
        try
        {
          return typeSet [eventType];
        }
        catch (Exception e)
        {
          throw new InvalidOperationException ($"Cannot find channel for event type {eventType}.", e);
        }
      }
    }

    public void Add (EventChannel eventChannel)
    {
      try
      {
        typeSet.Add (eventChannel.GetEventType (), eventChannel);
        Set.Add (eventChannel);
      }
      catch (Exception e)
      {
        throw new InvalidOperationException ($"Cannot add channel for event type {eventChannel.GetEventType ()}.", e);
      }
    }

    public void Remove<TEvent> () where TEvent : IEvent
    {
      Remove (typeof(TEvent));
    }

    public void Remove (Type eventType)
    {
      Remove (this [eventType]);
    }

    public void Remove (EventChannel eventChannel)
    {
      Set.Remove (eventChannel);
      typeSet.Remove (eventChannel.GetEventType ());

      eventChannel.UnsubscribeAll ();
    }

    public void RemoveWhere (Func<EventChannel, bool> condition)
    {
      Set.Where (condition, Remove);
    }

    public void RemoveBy (IEventsContext context)
    {
      RemoveWhere (channel => context.Equals (channel.Context));
    }

    public bool TryGet (Type eventType, out EventChannel eventChannel)
    {
      return typeSet.TryGetValue (eventType, out eventChannel);
    }

    public virtual void Unsubscribe (IEventsHandler handler)
    {
      foreach (var eventChannel in Set)
      foreach ((int index, EventsHandler callback) in eventChannel.Handlers.WithIndex ())
        if (callback.IsTarget (handler))
          eventChannel.Handlers.RemoveAt (index);
    }

    public virtual void UnsubscribeAll ()
    {
      Set.ForEach (e => e.UnsubscribeAll ());
    }

    public virtual void Dispose ()
    {
      Set.ForEach (Remove);
      typeSet.Clear ();
    }
  }
}