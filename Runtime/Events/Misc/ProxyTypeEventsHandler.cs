using System;
using System.Reflection;

namespace Arunoki.Flow.Misc
{
  internal class ProxyTypeEventsHandler : EventsHandler
  {
    public ProxyTypeEventsHandler (Type type, MethodInfo [] methods)
      : base (type, methods)
    {
    }

    public override bool IsTarget (object other)
    {
      return other is Type && Target == other;
    }

    public override object GetTargetInstance () => null;
  }
}