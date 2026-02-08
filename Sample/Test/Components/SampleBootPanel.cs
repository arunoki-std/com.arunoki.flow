using Arunoki.Flow.Sample.Events;
using Arunoki.Flow.Sample.Managers;

using System.Collections;

using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace Arunoki.Flow.Sample
{
  [Preserve]
  [DisallowMultipleComponent]
  public sealed class SampleBootPanel : MonoBehaviour, IHandler
  {
    [SerializeField] private Text percents;
    [SerializeField] private Text completed;
    [SerializeField] private Slider progress;

    private IEnumerator Start ()
    {
      while (!SampleManager.Boot.IsStarted)
        yield return null;

      SampleHub.Get.Produce (this);

      SetProgress (0);
      SetReady (false);
    }

    private void OnDestroy ()
    {
      SampleHub.Get.Clear (this);
    }

    public void OnProgressChanged (ref BootstrapProgress e)
    {
      SetProgress (e.Value);
    }

    public void OnBootstrapReady (ref BootstrapReady e)
    {
      SetReady (true);
    }

    private void SetProgress (float value)
    {
      progress.value = value;
      percents.text = $"{Mathf.CeilToInt (value * 100.0f)} %";
    }

    private void SetReady (bool isCompleted)
    {
      completed.gameObject.SetActive (isCompleted);
      progress.gameObject.SetActive (!isCompleted);
      percents.gameObject.SetActive (!isCompleted);
    }
  }
}