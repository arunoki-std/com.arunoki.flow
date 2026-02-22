#nullable enable

using System;

namespace Arunoki.Flow.Utilities
{
  public static class Guard
  {
    public static bool IsNull (object? value)
      => value is null || (value is UnityEngine.Object uo && uo == null);

    public static void ThrowIfRewrite (object? current, object? other)
    {
      if (current == null || other == null) return;
      throw new RewriteOperationException (current, other);
    }

    public static void ThrowIfNotASubclass (Type target, Type parent)
    {
      if (!target.IsSubclassOf (parent))
        throw new InvalidOperationException ($"Type '{target}' must be a subclass of '{parent}'.");
    }

    public static void ThrowIfNotAssignable (Type target, Type implementation)
    {
      if (!implementation.IsAssignableFrom (target))
        throw new InvalidOperationException ($"Class '{target}' doesn't implement '{implementation}'.");
    }
  }
}