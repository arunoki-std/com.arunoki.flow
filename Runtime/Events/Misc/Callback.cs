using System;

namespace Arunoki.Flow.Misc
{
  public abstract class Callback
  {
    private readonly bool isTargetStaticManager;
    protected object EventTarget;

    protected Callback (object eventTarget)
    {
      EventTarget = eventTarget;
      isTargetStaticManager = eventTarget is Type;
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
    public virtual object GetTargetInstance () => isTargetStaticManager ? null : EventTarget;

    public virtual bool IsActive ()
    {
      return GetTargetInstance () is not IActiveHandler handler || handler.IsHandlingEvents;
    }
  }
}