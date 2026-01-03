using Arunoki.Flow.Samples.Controllers;
using Arunoki.Flow.Samples.Events;

using UnityEngine;
using UnityEngine.Scripting;

namespace Arunoki.Flow.Samples
{
  public partial class Battery
  {
    [Preserve]
    public class PowerHandler : BaseBatteryHandler<Battery>
    {
      public void OnChanged (PowerEvent power)
      {
        if (Mathf.Approximately (power.Value, 1.0f))
          Battery.Charged ();

        else if (power.Value < 0.99f) Battery.NotCharged ();

        else if (power.Value > 1.0f) Battery.Overload.Call ();
      }
    }
  }
}