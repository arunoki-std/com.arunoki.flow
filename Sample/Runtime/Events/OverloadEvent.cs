namespace Arunoki.Flow.Sample.Events
{
  public struct OverloadEvent : IDomainEvent
  {
    public IContext Context { get; set; }

    public string Message { get; set; }
  }
}