using Arunoki.Flow.Sample.Events;
using Arunoki.Flow.Sample.Managers;
using UnityEngine.Scripting;

namespace Arunoki.Flow.Sample
{
    [Preserve]
    public partial class SampleFlowPipeline : IFlowPipeline
    {
        /// Won't be invoked, class doesn't implement <see cref="IPipelineHandler"/>
        public void OnBootReady(ref BootstrapReady e)
        {
            SampleManager.Log<SampleFlowPipeline>(e);
        }
    }
}
