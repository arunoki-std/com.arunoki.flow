namespace Arunoki.Flow
{
  public class Bool<TEvent> : Property<TEvent, bool> where TEvent : struct, IValueEvent<bool>
  {
    public static implicit operator bool (Bool<TEvent> a) => a.Value;
  }
}