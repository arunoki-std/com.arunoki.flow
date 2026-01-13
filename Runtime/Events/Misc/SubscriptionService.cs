using Arunoki.Collections;

using System;

namespace Arunoki.Flow.Misc
{
  public class SubscriptionService : Set<Callback>, IService
  {
    public SubscriptionService (EventBus events)
    {
      Events = events;
    }

    protected EventBus Events { get; }

    public bool IsActive { get; private set; }

    public virtual void Subscribe (Type staticHandler)
    {
      AddRange (Events.Subscribe (staticHandler).ToArray ());
    }

    public virtual void Subscribe (IHandler handler)
    {
      AddRange (Events.Subscribe (handler).ToArray ());
    }

    protected override void OnElementAdded (Callback callback)
    {
      base.OnElementAdded (callback);

      if (!IsActive) Unsubscribe (callback);
    }

    public virtual void Unsubscribe (IHandler handler)
    {
      Events.Unsubscribe (handler);
    }

    protected void Unsubscribe (Callback callback)
    {
      Events [callback.EventType].Remove (callback);
    }

    public void Activate ()
    {
      if (IsActive) return;
      IsActive = true;

      foreach (var callback in Elements)
        Events [callback.EventType].Add (callback);
    }

    public void Deactivate ()
    {
      if (!IsActive) return;
      IsActive = false;

      foreach (var callback in Elements)
        Unsubscribe (callback);
    }
  }
}