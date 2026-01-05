using Arunoki.Collections;
using Arunoki.Flow.Misc;
using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow
{
  public abstract class EventChannel : Set<Callback>, IEventChannel, IContextPart
  {
    private readonly Type eventType;

    public IContext Context { get; private set; }

    protected EventChannel (Type eventType)
    {
      this.eventType = eventType;
    }

    protected virtual void Publish<TE> (ref TE evt) where TE : struct, IEvent
    {
      foreach (var handler in Elements)
        if (handler.IsActive ())
          handler.Publish (ref evt);
    }

    public Type GetEventType () => eventType;

    IContext IContextPart.Get () => Context;

    void IContextPart.Set (IContext context)
    {
      if (Utils.IsDebug ())
      {
        if (Context != null && context != null)
          throw new InvalidOperationException (
            $"Trying to rewrite existing {nameof(Context)} '{Context}' by '{context}'.");
      }

      Context = context;
    }
  }
}