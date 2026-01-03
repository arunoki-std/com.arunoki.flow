using UnityEngine;

namespace Arunoki.Flow.Sample
{
  public class BatterySampleBehaviour : MonoBehaviour
  {
    [Range (0, 2.5f)] public float chargeValue = 0.3f;
    [Range (0, 2.5f)] public float dischargeValue = 0.1f;

    private Battery battery;

    private void Awake ()
    {
      battery = new Battery ();
    }

    private void OnEnable ()
    {
      battery.Reset ();
    }

    private void OnDisable ()
    {
      battery.Dispose ();
    }

    private void Update ()
    {
      battery.Power.Set (Mathf.Min (battery.Power.Value + chargeValue, 1.1f));
      battery.Power.Set (Mathf.Max (battery.Power.Value - dischargeValue, 0.0f));
      if (battery.IsCharged) this.enabled = false;
    }
  }
}