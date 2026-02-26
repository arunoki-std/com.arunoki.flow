using UnityEngine;

namespace Arunoki.Flow.Sample
{
  public class FsmEntityController : MonoBehaviour
  {
    private FlowHub hub;

    private FsmEntity entity;

    private void Awake ()
    {
      entity = new FsmEntity ();
      hub = new FlowHub (entity);
      hub.Initialize ();
    }

    private void OnEnable ()
    {
      UnityEngine.Debug.Log ($"\n\n");
      UnityEngine.Debug.Log ($"\t ACTIVATE");
      hub.Activate ();
    }

    private void OnDisable ()
    {
      UnityEngine.Debug.Log ($"\t DEACTIVATE");
      hub.Deactivate ();
    }

    private void Update ()
    {
      entity.Update ();
    }
  }
}