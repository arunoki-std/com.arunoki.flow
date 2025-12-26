using System;

namespace Arunoki.Flow.Misc
{
  internal readonly struct ProxyTypeContext : IEventsContext
  {
    public readonly Type Type;

    public ProxyTypeContext (Type type)
    {
      Type = type;
    }

    public override string ToString () => $"WrapperOf({Type})";
  }
}