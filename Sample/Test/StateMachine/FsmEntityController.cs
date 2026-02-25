using UnityEngine;

namespace Arunoki.Flow.Sample
{
  public class FsmEntityController : MonoBehaviour
  {
    private FlowHub hub;
    private StateMachine<FsmEntity> stateMachine;

    private FsmEntity entity;

    private void Awake ()
    {
      entity = new FsmEntity ();
      hub = new FlowHub (entity);
      stateMachine = new StateMachine<FsmEntity> (entity, hub);
      hub.Services.Register (stateMachine);
    }

    private void Start ()
    {
      hub.Activate ();
    }

    private void Update ()
    {
      stateMachine.Update ();
    }
  }
}