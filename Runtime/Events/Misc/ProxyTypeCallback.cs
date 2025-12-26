using System;
using System.Reflection;

namespace Arunoki.Flow.Misc
{
  internal class ProxyTypeCallback : Callback
  {
    public ProxyTypeCallback (Type type, MethodInfo [] methods)
      : base (type, methods)
    {
    }

    public override bool IsReceiver (object target)
    {
      return target is Type && Receiver == target;
    }

    public override object GetTargetInstance () => null;
  }
}