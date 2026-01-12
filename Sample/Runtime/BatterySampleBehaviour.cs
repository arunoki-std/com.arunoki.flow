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
      battery.Hub.Activate ();
    }

    private void OnDisable ()
    {
      battery.Hub.Deactivate ();
      battery.Power.Set (0.333333333f);
      UnityEngine.Debug.LogWarning ("DEACTIVATED");
    }

    private void Update ()
    {
      battery.Power.Set (Mathf.Min (battery.Power.Value + (chargeValue * Random.Range (0.5f, 1.0f)), 1.1f));

      if (battery.IsCharged) this.enabled = false;
    }
  }
}