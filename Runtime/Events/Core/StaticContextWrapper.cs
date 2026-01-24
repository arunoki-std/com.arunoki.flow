using System;

namespace Arunoki.Flow.Core
{
  internal readonly struct StaticContextWrapper : IContext
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