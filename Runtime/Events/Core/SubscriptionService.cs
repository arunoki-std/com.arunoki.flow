using Arunoki.Collections;
using Arunoki.Flow.Utilities;

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

    public bool IsActive () => isActivated;

    public virtual void Register (Type staticHandler)
    {
      Callbacks.AddRange (Events.Subscribe (staticHandler).ToArray ());
    }

    public virtual void Register (IFlowHandler handler)
    {
      Callbacks.AddRange (Events.Subscribe (handler).ToArray ());
    }

    public virtual void Remove (IFlowHandler handler)
    {
      Events.Unsubscribe (handler);
    }

    protected override void OnElementAdded (Callback callback)
    {
      base.OnElementAdded (callback);

      if (!isActivated) callback.Deactivate (Events);
    }

    public void Activate ()
    {
      if (isActivated) return;
      isActivated = true;

      for (var index = 0; index < Callbacks.Count; index++)
        Callbacks [index].Activate (Events);
    }

    public void Deactivate ()
    {
      if (!isActivated) return;
      isActivated = false;

      for (var index = 0; index < Callbacks.Count; index++)
        Callbacks [index].Deactivate (Events);
    }
  }
}