namespace Arunoki.Flow.Samples.Events
{
  public struct ChargeStatusEvent : IValueEvent<bool>
  {
    public ChargeStatusEvent (IContext context, bool value, bool previous)
    {
      Context = context;
      Value = value;
      Previous = previous;
    }

    public IContext Context { get; }
    public bool Value { get; }
    public bool Previous { get; }
  }
}