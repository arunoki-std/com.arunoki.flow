using System;

namespace Arunoki.Flow.Misc
{
  public abstract class Callback
  {
    public Type EventType { get; }
    protected readonly bool IsTargetStatic;
    protected readonly object EventTarget;

    protected Callback (object eventTarget, Type eventType)
    {
      EventType = eventType;
      EventTarget = eventTarget;
      IsTargetStatic = eventTarget is Type;
    }

    /// Define whether is eventTarget is methods source. 
    public virtual bool IsConsumable (object eventTarget)
    {
      return EventTarget == eventTarget;
    }

    /// Subscriber instance (null if subscriber is static manager).
    public virtual object GetTargetInstance () => IsTargetStatic ? null : EventTarget;

    public virtual bool IsTargetReceivingEvents ()
    {
      return GetTargetInstance () is not IActiveHandler handler || handler.IsHandlingEvents;
    }
  }
}