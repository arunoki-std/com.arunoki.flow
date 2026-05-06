using System;

namespace Arunoki.Flow.Events.Core
{
  internal readonly struct StaticContextWrapper : IFlowContext
  {
    public readonly Type StaticType;

    public StaticContextWrapper (Type staticType)
    {
      StaticType = staticType;
    }

    public bool IsConsumable (Type other) => StaticType == other;

    public override string ToString () => $"WrapperOf({StaticType})";
  }
}