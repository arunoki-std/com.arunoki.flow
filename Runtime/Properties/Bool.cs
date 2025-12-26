namespace Arunoki.Flow
{
  public class Bool<TEvent> : Property<TEvent, bool> where TEvent : IValueEvent<bool>, new ()
  {
    public static implicit operator bool (Bool<TEvent> a) => a.Value;
  }
}