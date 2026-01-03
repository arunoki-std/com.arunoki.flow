using Arunoki.Collections;
using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  public class EventChannel : IEventChannel
  {
    private readonly Type eventType;

    protected internal Set<EventsHandler> Handlers = new();

    public EventChannel (Type eventType)
    {
      this.eventType = eventType;
    }

    public IContext Context { get; private set; }

    public void Subscribe (EventsHandler eventsHandler)
    {
      Handlers.Add (eventsHandler);
    }

    public void Unsubscribe (EventsHandler eventsHandler)
    {
      Handlers.Remove (eventsHandler);
    }

    protected internal virtual void Call (ref object evt)
    {
      foreach (var handler in Handlers)
        if (handler.IsActive ())
          handler.OnCallback (ref evt);
    }

    public virtual void UnsubscribeAll ()
    {
      Handlers.Clear ();
    }

    protected internal virtual void InitContext (IContext context) => Context ??= context;

    public Type GetEventType () => eventType;
  }
}