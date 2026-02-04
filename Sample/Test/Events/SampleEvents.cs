namespace Arunoki.Flow.Sample.Events
{
  public struct SampleContextFired : IDomainEvent
  {
    public IContext Context { get; set; }
  }
}