namespace Arunoki.Flow.Sample
{
  public struct TimeoutEvent : IDomainEvent
  {
    public IContext Context { get; set; }
  }
}