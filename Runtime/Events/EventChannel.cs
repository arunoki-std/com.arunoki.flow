using Arunoki.Collections;
using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  public class EventChannel : IEventChannel
  {
    private readonly Type eventType;

    protected internal Set<Callback> Callbacks = new();

    public EventChannel (Type eventType)
    {
      this.eventType = eventType;
    }

    public IEventsContext Context { get; private set; }

    public void Subscribe (Callback callback)
    {
      Callbacks.Add (callback);
    }

    public void Unsubscribe (Callback callback)
    {
      Callbacks.Remove (callback);
    }

    protected internal virtual void Call (object message)
    {
      Callbacks.ForEach (callback => callback.OnCallback (message));
    }

    public virtual void UnsubscribeAll ()
    {
      Callbacks.ForEach (Unsubscribe);
    }

    protected internal virtual void InitContext (IEventsContext context) => Context ??= context;

    public Type GetEventType () => eventType;
  }
}