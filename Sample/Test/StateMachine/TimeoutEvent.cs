namespace Arunoki.Flow.Sample
{
  public struct TimeoutEvent : IDomainEvent
  {
    public IFlowContext Context { get; set; }
  }
}