using System;

using UnityEngine;

namespace Arunoki.Flow.Basics
{
  [DisallowMultipleComponent]
  public sealed class RoutineHelper : MonoBehaviour
  {
    public event Action OnFrameUpdate = delegate { };
    public event Action OnLateUpdate = delegate { };
    public event Action OnFixedUpdate = delegate { };


    private void Update ()
    {
      OnFrameUpdate ();
    }

    private void LateUpdate ()
    {
      OnLateUpdate ();
    }

    private void FixedUpdate ()
    {
      OnFixedUpdate ();
    }

    private void OnDisable ()
    {
      StopAllCoroutines ();
    }

    private void OnDestroy ()
    {
      OnFrameUpdate = null;
      OnFixedUpdate = null;
      OnLateUpdate = null;
    }
  }
}