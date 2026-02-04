using Arunoki.Flow.Sample.Events;
using Arunoki.Flow.Sample.Managers;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample
{
  public partial class SampleContext
  {
    [Preserve]
    public class Fire : PipelineHandler
    {
      public void OnFired (ref SampleContextFired e)
      {
        SampleManager.Log<Fire> (e);
      }
    }
  }
}