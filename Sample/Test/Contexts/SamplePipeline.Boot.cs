using Arunoki.Flow.Sample.Events;
using Arunoki.Flow.Sample.Managers;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample
{
  public partial class SamplePipeline
  {
    [Preserve]
    public class Boot : IPipelineHandler
    {
      public void OnStarted (ref BootstrapStarted e)
      {
        SampleManager.Log<Boot> (e);
      }

      public void OnCompleted (ref BootstrapCompleted e)
      {
        SampleManager.Log<Boot> (e);
      }
    }
  }
}