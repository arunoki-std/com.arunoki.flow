using System.Collections.Generic;
using System.Linq;

namespace Arunoki.Flow
{
  internal static partial class Globals
  {
    public static string JoinAsList (IEnumerable<object> items)
    {
      return $"[{string.Join (", ", items.Select<object, object> (e => e?.ToString ()))}]";
    }
  }
}