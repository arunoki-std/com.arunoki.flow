#nullable enable

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
  }
}