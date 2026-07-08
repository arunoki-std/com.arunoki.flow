using Arunoki.Flow.Sample.Events;

namespace Arunoki.Flow.Sample
{
    public class SampleBootModel : IFlowContext
    {
        public Trigger<BootstrapStarted> IsStarted { get; } = new(false);
        public Trigger<BootstrapCompleted> IsCompleted { get; } = new(false);
        public Trigger<BootstrapReady> IsReady { get; } = new(false);
        public ProgressProperty<BootstrapProgress> Progress { get; } = new(false);
    }
}
