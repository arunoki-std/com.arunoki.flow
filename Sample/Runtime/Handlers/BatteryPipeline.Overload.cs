using Arunoki.Flow.Sample.Events;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample.Handlers
{
  internal sealed partial class BatteryPipeline
  {
    [Preserve]
    internal sealed class Overload : BatteryHandler
    {
      public void OnEvent (ref OverloadEvent evt)
      {
        UnityEngine.Debug.Log ($"{nameof(Overload)}");
      }
    }
  }
}