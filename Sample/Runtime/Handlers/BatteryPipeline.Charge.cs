using Arunoki.Flow.Sample.Events;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample.Handlers
{
  internal sealed partial class BatteryPipeline
  {
    [Preserve]
    internal sealed class Charge : BatteryHandler
    {
      public void OnProgress (ref PowerEvent evt)
      {
        UnityEngine.Debug.Log ($"{nameof(Charge)} progress : {evt.Value}");
      }

      public void OnCharged (ref ChargeStatusEvent evt)
      {
        UnityEngine.Debug.Log ($"{nameof(Charge)} status: {evt.Value}");
      }
    }
  }
}