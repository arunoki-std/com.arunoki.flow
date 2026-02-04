using Arunoki.Flow.Sample.Events;
using Arunoki.Flow.Sample.Managers;

using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample
{
  [Preserve]
  public partial class SamplePipeline : IPipeline
  {
    /// Won't be invoked, class doesn't implement <see cref="IPipelineHandler"/> 
    public void OnBootReady (ref BootstrapReady e)
    {
      SampleManager.Log<SamplePipeline> (e);
    }
  }
}