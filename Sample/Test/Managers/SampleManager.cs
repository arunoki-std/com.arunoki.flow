using Arunoki.Flow.Sample.Events;

using JetBrains.Annotations;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample.Managers
{
  [Preserve]
  public static partial class SampleManager
  {
    public static SampleBootModel Boot { get; } = new();
    public static SampleContext Context { get; } = new();

    static SampleManager ()
    {
      SampleHub.Get.OnReady += () =>
      {
        SampleHub.Get.Produce (new SamplePipeline ());
      };
    }

    [UsedImplicitly]
    private static void OnBootStarted (ref BootstrapStarted e)
    {
      Log (typeof(SampleManager), e);
    }

    [UsedImplicitly]
    private static void OnBootCompleted (ref BootstrapCompleted e)
    {
      Log (typeof(SampleManager), e);
    }

    [UsedImplicitly]
    private static void OnContextFired (ref SampleContextFired e)
    {
      Log (typeof(SampleManager), e);
    }
  }
}