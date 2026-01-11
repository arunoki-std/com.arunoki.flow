using System;
using System.Reflection;

namespace Arunoki.Flow.Misc
{
  public class Callback<TEvent> : Callback where TEvent : struct, IEvent
  {
    private readonly RefActionEvent<TEvent> [] invokers;

    public Callback (object eventTarget, Type eventType, MethodInfo [] methods) : base (eventTarget, eventType)
    {
      invokers = new RefActionEvent<TEvent>[methods.Length];

      var i = 0;

      try
      {
        for (; i < methods.Length; i++)
        {
          invokers [i] = IsTargetStatic
            ? (RefActionEvent<TEvent>) methods [i].CreateDelegate (typeof(RefActionEvent<TEvent>))
            : (RefActionEvent<TEvent>) methods [i].CreateDelegate (typeof(RefActionEvent<TEvent>), eventTarget);
        }
      }
      catch (ArgumentException)
      {
        throw new IncompatibleEventHandlerException<TEvent> (eventTarget, methods [i]);
      }
    }


    public void Publish (ref TEvent evt)
    {
      for (var i = 0; i < invokers.Length; i++)
        invokers [i] (ref evt);
    }

    public override void Dispose ()
    {
      base.Dispose ();

      for (var i = 0; i < invokers.Length; i++)
        invokers [i] = null;
    }
  }
}