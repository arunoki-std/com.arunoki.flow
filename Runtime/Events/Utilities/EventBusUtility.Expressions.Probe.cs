#nullable enable

using Arunoki.Flow.Events;

using System;
using System.Linq.Expressions;

namespace Arunoki.Flow.Utilities
{
  internal static partial class EventBusUtility
  {
    /// 0 = unknown, 1 = supported, -1 = not supported
    private static int _expressionState;

    public static bool IsExpressionSupported ()
    {
      if (_expressionState != 0) return _expressionState > 0;

      try
      {
        {
          // check pattern "object -> (DeclaringType) -> property"
          var src = Expression.Parameter (typeof(object), "src");
          var cast = Expression.Convert (src, typeof(ExpressionProbeSource));
          var prop = Expression.Property (cast, nameof(ExpressionProbeSource.Channel));
          var toBase = Expression.Convert (prop, typeof(Channel));
          Expression.Lambda<Func<object, Channel>> (toBase, src).Compile ();
        }

        _expressionState = 1;
      }
      catch (PlatformNotSupportedException) { _expressionState = -1; }
      catch (NotSupportedException) { _expressionState = -1; }
      catch (InvalidOperationException) { _expressionState = -1; }

      return _expressionState > 0;
    }

    private sealed class ExpressionProbeSource
    {
      public Channel Channel => null!;
    }
  }
}