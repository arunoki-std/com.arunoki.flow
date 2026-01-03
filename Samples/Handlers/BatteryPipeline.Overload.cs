using Arunoki.Flow.Samples.Events;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Samples.Controllers
{
  internal sealed partial class BatteryPipeline
  {
    [Preserve]
    internal sealed class Overload : BaseBatteryHandler<Battery>
    {
      public void OnEvent (OverloadEvent evt)
      {
        UnityEngine.Debug.Log ($"{nameof(Overload)}");
      }
    }
  }
}