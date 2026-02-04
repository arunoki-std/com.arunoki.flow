using Arunoki.Flow.Globals;
using Arunoki.Flow.Sample.Managers;

using System.Collections;

using UnityEngine;

namespace Arunoki.Flow.Sample.Controllers
{
  public sealed class SampleBootController : MonoBehaviour
  {
    [Range (1, 10)] [SerializeField] private float secondsToComplete = 3.0f;

    private void Awake ()
    {
      var hub = new SampleHub ();
      GlobalHub.Init (hub, new StaticBootstrap (typeof(SampleManager)));
    }

    private IEnumerator Start ()
    {
      var boot = SampleManager.Boot;

      boot.IsStarted.Set ();
      yield return null;

      var t = 0.0f;
      while (!boot.Progress.IsReady ())
      {
        t += Time.deltaTime;
        boot.Progress.Set (t / secondsToComplete);
        yield return null;
      }

      boot.IsCompleted.Set ();
      yield return new WaitForSeconds (1.0f);

      boot.IsReady.Set ();
    }
  }
}