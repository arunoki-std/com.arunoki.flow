using System;
using System.Collections;

using UnityEngine;

namespace Arunoki.Flow.Basics
{
  [DisallowMultipleComponent]
  public sealed class RoutineHelper : MonoBehaviour
  {
    private readonly ActiveOperations activeOperations = new("Helper");

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
      activeOperations.Clear ();
    }

    private void OnDestroy ()
    {
      OnFrameUpdate = null;
      OnFixedUpdate = null;
      OnLateUpdate = null;
    }

    public Coroutine StartTrackedCoroutine
      (IEnumerator routine, string operationName, ActiveOperations operations = null)
      => StartCoroutine (TrackedCoroutine (routine, operationName, operations ?? GetOperations ()));

    private static IEnumerator TrackedCoroutine
      (IEnumerator routine, string operationName, ActiveOperations operations)
    {
      if (string.IsNullOrEmpty (operationName))
        throw new ArgumentNullException (nameof(operationName));

      uint operationId = operations.Track (operationName);

      try
      {
        while (routine.MoveNext ())
          yield return routine.Current;
      }
      finally
      {
        operations.Untrack (operationId);
      }
    }

    public ActiveOperations GetOperations () => activeOperations;
  }
}