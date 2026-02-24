using System;

namespace Arunoki.Flow.Sample.States
{
  public class BaseState : State<FsmEntity>
  {
    protected BaseState (bool isDefault, Type parentType) : base (isDefault, parentType)
    {
    }

    public override void OnEnter ()
    {
      UnityEngine.Debug.LogWarning ($"Entered:\t ({GetType ().Name})");
    }

    public override void OnExit ()
    {
      UnityEngine.Debug.LogWarning ($"Exit:\t ({GetType ().Name})");
    }

    public override void OnUpdate ()
    {
    }
  }
}