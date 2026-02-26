using System;

using UnityEngine;

namespace Arunoki.Flow.Sample.States
{
  public abstract class BaseStateTimeout : BaseState
  {
    protected float TimeMin = 2;
    protected float TimeMax = 4;

    private float timeElapsed;

    protected BaseStateTimeout (bool isDefault, Type parentType)
      : base (isDefault, parentType)
    {
    }

    public override void OnEnter ()
    {
      base.OnEnter ();

      timeElapsed = UnityEngine.Random.Range (TimeMin, TimeMax);
    }

    public override void OnUpdate ()
    {
      base.OnUpdate ();

      timeElapsed -= Time.deltaTime;

      if (timeElapsed <= TimeMin)
        Entity.Timeout.Emit ();
    }
  }
}