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

    private void Start ()
    {
      // hub.Activate ();
    }

    private void Update ()
    {
      // entity.Update ();
    }
  }
}