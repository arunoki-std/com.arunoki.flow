using System;
using System.Reflection;

namespace Arunoki.Flow
{
  public class IncompatibleEventHandlerException<TEvent> : Exception where TEvent : struct, IEvent
  {
    public IncompatibleEventHandlerException (object target, MethodInfo receiver)
      : base (
        $"Method '{target}.{receiver.Name}({receiver.GetParameters () [0].ParameterType})' cannot be invoked on event {typeof(TEvent).Name}. " +
        $"Method method must have (ref TEvent evt) signature.")
    {
    }
  }
}