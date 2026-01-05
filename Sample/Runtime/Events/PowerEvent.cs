namespace Arunoki.Flow.Sample.Events
{
  public struct PowerEvent : IValueEvent<float>
  {
    public IContext Context { get; set; }
    public float Value { get; set; }
    public float Previous { get; set; }
  }
}