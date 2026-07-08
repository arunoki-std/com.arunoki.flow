namespace Arunoki.Flow.Sample.Events
{
    public struct BootstrapStarted : IDomainEvent
    {
        public IFlowContext Context { get; set; }
    }

    public struct BootstrapCompleted : IDomainEvent
    {
        public IFlowContext Context { get; set; }
    }

    public struct BootstrapReady : IDomainEvent
    {
        public IFlowContext Context { get; set; }
    }

    public struct BootstrapProgress : IValueEvent<float>
    {
        public IFlowContext Context { get; set; }
        public float Current { get; set; }
        public float Previous { get; set; }
    }
}
