using System;
using System.Reflection;

namespace Arunoki.Flow.Misc
{
  public class EventsHandler
  {
    protected MethodInfo [] Methods;

    public EventsHandler (object target, MethodInfo [] methods)
    {
      Target = target;
      Methods = methods;
    }

    public object Target { get; private set; }

    public virtual void Publish<TE> (ref TE evt) where TE : struct, IEvent
    {
    }

    [Obsolete ("use Publish() instead")]
    public void OnCallback (ref object message)
    {
      var target = GetTargetInstance ();

      for (var i = 0; i < Methods.Length; i++)
        Methods [i].Invoke (target, new [] { message });
    }

    public void Dispose ()
    {
      Methods = null;
      Target = null;
    }

    public virtual bool IsTarget (object other)
    {
      return other is IHandler && Target == other;
    }

    public virtual object GetTargetInstance () => Target;

    public virtual bool IsActive ()
    {
      return GetTargetInstance () is not IActiveHandler handler || handler.IsHandlingEvents;
    }
  }
}