using System;

namespace Arunoki.Flow.Misc
{
  public abstract class Callback
  {
    protected readonly bool IsTargetStatic;
    protected object EventTarget;

    protected Callback (object eventTarget)
    {
      EventTarget = eventTarget;
      IsTargetStatic = eventTarget is Type;
    }

    public virtual void Dispose ()
    {
      EventTarget = null;
    }

    public virtual bool IsConsumable (object eventTarget)
    {
      return eventTarget is IHandler && EventTarget == eventTarget;
    }

    /// Subscriber instance (null if subscriber is static manager).
    public virtual object GetTargetInstance () => IsTargetStatic ? null : EventTarget;

    public virtual bool IsActive ()
    {
      return GetTargetInstance () is not IActiveHandler handler || handler.IsHandlingEvents;
    }
  }
}