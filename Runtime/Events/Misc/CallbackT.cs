using System;
using System.Reflection;

namespace Arunoki.Flow.Misc
{
  public class Callback<TEvent> : Callback where TEvent : struct, IEvent
  {
    private readonly RefAction<object, TEvent> [] invokers;

    public Callback (object eventTarget, MethodInfo [] methods) : base (eventTarget)
    {
      invokers = new RefAction<object, TEvent>[methods.Length];
      var i = 0;
      try
      {
        for (; i < methods.Length; i++)
          invokers [i] = (RefAction<object, TEvent>) methods [i].CreateDelegate (typeof(RefAction<object, TEvent>));
      }
      catch (ArgumentException)
      {
        throw new IncompatibleEventHandlerException<TEvent> (eventTarget, methods [i]);
      }
    }

    public void Publish (ref TEvent evt)
    {
      var obj = GetTargetInstance ();

      for (var i = 0; i < invokers.Length; i++)
        invokers [i] (obj, ref evt);
    }

    public override void Dispose ()
    {
      base.Dispose ();

      for (var i = 0; i < invokers.Length; i++)
        invokers [i] = null;
    }
  }
}