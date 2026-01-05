using Arunoki.Collections;
using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  public abstract class EventChannel : IEventChannel
  {
    private readonly Type eventType;

    protected internal Set<EventsHandler> Handlers { get; } = new Set<EventsHandler> ();

    protected EventChannel (Type eventType)
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

    protected virtual void Publish<TE> (ref TE evt) where TE : struct, IEvent
    {
      foreach (var handler in Handlers)
        if (handler.IsActive ())
          handler.Publish (ref evt);
    }

    [Obsolete ("use Publish() instead", true)]
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