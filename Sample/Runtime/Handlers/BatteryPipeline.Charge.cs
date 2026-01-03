using Arunoki.Flow.Sample.Events;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample.Controllers
{
  internal sealed partial class BatteryPipeline
  {
    [Preserve]
    internal sealed class Charge : BaseBatteryHandler<Battery>
    {
      public void OnProgress (PowerEvent evt)
      {
        UnityEngine.Debug.Log ($"{nameof(Charge)} progress : {evt.Value}");
      }

      public void OnCharged (ChargeStatusEvent evt)
      {
        UnityEngine.Debug.Log ($"{nameof(Charge)} status: {evt.Value}");
      }
    }
  }
}