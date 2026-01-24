using Arunoki.Collections;
using Arunoki.Flow.Misc;
using Arunoki.Flow.Utilities;

using System;
using System.Reflection;

namespace Arunoki.Flow
{
  public abstract class Channel : Set<Callback>, IEventChannel, IContextPart
  {
    private readonly Type eventType;

    public IContext Context { get; private set; }

    protected Channel (Type eventType)
    {
      this.eventType = eventType;
    }

    protected internal abstract Callback Subscribe (object target, MethodInfo [] methods);

    public Type GetEventType () => eventType;

    IContext IContextPart.Get () => Context;

    void IContextPart.Set (IContext context)
    {
      if (Utils.IsDebug ())
      {
        if (Context != null && context != null)
          throw new InvalidOperationException (
            $"Trying to rewrite existing {nameof(Context)} '{Context}' by other '{context}'.");
      }

      Context = context;
    }
  }
}