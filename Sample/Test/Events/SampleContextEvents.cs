namespace Arunoki.Flow.Sample.Events
{
    public struct SampleContextFired : IDomainEvent
    {
        public IFlowContext Context { get; set; }
    }
}
