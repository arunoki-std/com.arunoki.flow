namespace Arunoki.Flow.Sample.Events
{
  public struct ChargeStatusEvent : IValueEvent<bool>
  {
    public IContext Context { get; set; }
    public bool Value { get; set; }
    public bool Previous { get; set; }
  }
}