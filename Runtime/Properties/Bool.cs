namespace Arunoki.Flow
{
  public class Bool<TEvent> : Property<TEvent, bool> where TEvent : struct, IValueEvent<bool>
  {
    public Bool (bool defaultValue = false) : base (defaultValue)
    {
    }

    public static implicit operator bool (Bool<TEvent> a) => a.Value;
  }
}