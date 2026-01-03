namespace Arunoki.Flow.Sample.Events
{
  public struct PowerEvent : IValueEvent<float>
  {
    public PowerEvent (IContext context, float value, float previous)
    {
      Context = context;
      Value = value;
      Previous = previous;
    }

    public IContext Context { get; }
    public float Value { get; }
    public float Previous { get; }
  }
}