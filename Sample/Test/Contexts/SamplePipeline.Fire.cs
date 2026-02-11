using Arunoki.Flow.Sample.Events;
using Arunoki.Flow.Sample.Managers;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample
{
  public partial class SamplePipeline
  {
    [Preserve]
    public class Fire : IHandler
    {
      public void OnTestFired (ref SampleContextFired e)
      {
        SampleManager.Log<Fire> (e);
      }
    }
  }
}