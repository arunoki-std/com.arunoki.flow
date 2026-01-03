using System.Collections.Generic;
using System.Linq;

namespace Arunoki.Flow.Utilities
{
  internal static partial class Utils
  {
    public static string JoinAsList (IEnumerable<object> items)
    {
      return $"[{string.Join (", ", items.Select<object, object> (e => e?.ToString ()))}]";
    }
  }
}