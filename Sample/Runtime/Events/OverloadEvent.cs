namespace Arunoki.Flow.Sample.Events
{
  public struct OverloadEvent : IDomainEvent, IContext
  {
    public IContext Context { get; set; }
  }
}