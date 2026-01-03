using UnityEngine;

namespace Arunoki.Flow.Sample
{
  public class BatterySampleBehaviour : MonoBehaviour
  {
    [Range (0, 2.5f)] public float chargeValue = 0.1f;
    [Range (0, 2.5f)] public float dischargeValue = 0.1f;

    private Battery battery;

    private void Awake ()
    {
      battery = new Battery ();
    }

    private void Start ()
    {
      battery.Power.Set (0.5f);
      battery.Power.Set (0.6f);
      battery.Power.Set (1.6f);
      UnityEngine.Debug.Log (battery.Power.Value);
    }

    private void OnEnable ()
    {
      battery.Reset ();
      battery.Power.Set (0.6f);
    }

    // private void Update ()
    // {
    //   battery.Power.Set (Mathf.Min (battery.Power.Value + chargeValue, 1.1f));
    //   battery.Power.Set (Mathf.Max (battery.Power.Value - dischargeValue, 0.0f));
    //   UnityEngine.Debug.Log (battery.Power.Value);
    // }
  }
}