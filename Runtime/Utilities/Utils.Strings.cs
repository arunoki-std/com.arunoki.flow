using System.Collections.Generic;
using System.Linq;

namespace Arunoki.Flow.Utilities
{
    public static partial class Utils
    {
        // debug/trace formatting — not a hot path
        public static string JoinAsList(IEnumerable<object> items)
        {
            return $"[{string.Join(", ", items.Select<object, object>(e => e?.ToString()))}]";
        }
    }
}
