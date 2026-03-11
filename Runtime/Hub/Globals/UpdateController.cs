using Arunoki.Flow.Builders;

using UnityEngine;

namespace Arunoki.Flow.Globals
{
  public sealed class UpdateController : MonoBehaviour
  {
    private UpdatableContainer updaters;

    private void Awake ()
    {
      UnityEngine.Object.DontDestroyOnLoad (gameObject);
      gameObject.hideFlags = HideFlags.NotEditable;
    }

    private void Start ()
    {
      updaters = GlobalHub.Instance.Updater;
    }

    private void Update ()
    {
      updaters.Update ();
    }

    private void LateUpdate ()
    {
      updaters.LateUpdate ();
    }

    private void FixedUpdate ()
    {
      updaters.FixedUpdate ();
    }
  }
}