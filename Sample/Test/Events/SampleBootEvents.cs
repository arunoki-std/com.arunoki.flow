namespace Arunoki.Flow.Sample.Events
{
  public struct BootstrapStarted : IDomainEvent
  {
    public IContext Context { get; set; }
  }

  public struct BootstrapCompleted : IDomainEvent
  {
    public IContext Context { get; set; }
  }
  
  public struct BootstrapReady : IDomainEvent
  {
    public IContext Context { get; set; }
  }

  public struct BootstrapProgress : IValueEvent<float>
  {
    public IContext Context { get; set; }
    public float Value { get; set; }
    public float Previous { get; set; }
  }
}