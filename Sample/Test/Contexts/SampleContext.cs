using Arunoki.Flow.Sample.Events;

namespace Arunoki.Flow.Sample
{
  public partial class SampleContext : IContext, IPipeline, IPipelineHandler
  {
    public Trigger<SampleContextFired> IsFired { get; } = new();

    public void OnBootReady (ref BootstrapReady e)
    {
      IsFired.Set ();
    }
  }
}