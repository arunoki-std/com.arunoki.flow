using Arunoki.Collections;
using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  public abstract class EventChannel : ContextPart, IEventChannel
  {
    private readonly Type eventType;

    protected internal Set<Callback> Callbacks { get; } = new();

    protected EventChannel (Type eventType)
    {
      this.eventType = eventType;
    }

    public void Subscribe (Callback callback)
    {
      Callbacks.Add (callback);
    }

    public void Unsubscribe (Callback callback)
    {
      Callbacks.Remove (callback);
    }

    protected virtual void Publish<TE> (ref TE evt) where TE : struct, IEvent
    {
      foreach (var handler in Callbacks)
        if (handler.IsActive ())
          handler.Publish (ref evt);
    }

    [Obsolete ("use Publish() instead", true)]
    protected internal virtual void Call (ref object evt)
    {
      foreach (var handler in Callbacks)
        if (handler.IsActive ())
          handler.OnCallback (ref evt);
    }

    public virtual void UnsubscribeAll ()
    {
      Callbacks.Clear ();
    }

    public Type GetEventType () => eventType;
  }
}