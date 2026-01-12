using Arunoki.Flow.Sample.Handlers;
using Arunoki.Flow.Sample.Events;

using UnityEngine;
using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample
{
  public partial class Battery
  {
    [Preserve]
    public class PowerHandler : BatteryHandler
    {
      public void OnChanged (ref PowerEvent power)
      {
        if (Mathf.Approximately (power.Value, 1.0f))
          Battery.Charged ();

        else if (power.Value < 0.99f) Battery.NotCharged ();

        else if (power.Value > 1.0f)
        {
          Battery.Overload.Publish ();
          Battery.Charged ();
        }
      }
    }
  }
}