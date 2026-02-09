using Arunoki.Collections;

using System;

namespace Arunoki.Flow.Events.Core
{
  internal class SubscriptionService : Container<Callback>, IService
  {
    private bool isActivated;

    internal readonly Set<Callback> Callbacks;

    public SubscriptionService (EventBus events) : base (null)
    {
      Events = events;
      Callbacks = new(this);
    }

    protected EventBus Events { get; }

    public bool IsActivated () => isActivated;

    public virtual void Register (Type staticHandler)
    {
      Callbacks.AddRange (Events.Subscribe (staticHandler).ToArray ());
    }

    public virtual void Register (IHandler handler)
    {
      Callbacks.AddRange (Events.Subscribe (handler).ToArray ());
    }

    public virtual void Remove (IHandler handler)
    {
      Events.Unsubscribe (handler);
    }

    protected void Remove (Callback callback)
    {
      Events [callback.EventType].Remove (callback);
    }

    protected override void OnElementAdded (Callback callback)
    {
      base.OnElementAdded (callback);

      if (!isActivated) Remove (callback);
    }

    public void Activate ()
    {
      if (isActivated) return;
      isActivated = true;

      foreach (var callback in Callbacks)
        Events [callback.EventType].TryAdd (callback);
    }

    public void Deactivate ()
    {
      if (!isActivated) return;
      isActivated = false;

      foreach (var callback in Callbacks)
        Remove (callback);
    }
  }
}