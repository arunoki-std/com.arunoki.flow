using Arunoki.Collections;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Misc
{
  public class EventChannelCollection
  {
    private readonly Dictionary<Type, EventChannel> eventsCache = new();

    protected internal Set<EventChannel> EventsGroup = new();

    public EventChannel this [Type eventType]
    {
      get
      {
        try
        {
          return eventsCache [eventType];
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
        eventsCache.Add (eventChannel.GetEventType (), eventChannel);
        EventsGroup.Add (eventChannel);
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
      EventsGroup.Remove (eventChannel);

      eventsCache.Remove (eventChannel.GetEventType ());
      eventChannel.UnsubscribeAll ();
    }

    public void Remove (Predicate<EventChannel> condition)
    {
      EventsGroup.ForEach (condition, Remove);
    }

    public void RemoveBy (IEventsContext context)
    {
      Remove (channel => context.Equals (channel.Context));
    }

    public bool TryGet (Type eventType, out EventChannel eventChannel)
    {
      return eventsCache.TryGetValue (eventType, out eventChannel);
    }

    public virtual void UnsubscribeAll ()
    {
      EventsGroup.ForEach (e => e.UnsubscribeAll ());
    }

    public virtual void Dispose ()
    {
      EventsGroup.ForEach (Remove);
      eventsCache.Clear ();
    }
  }
}