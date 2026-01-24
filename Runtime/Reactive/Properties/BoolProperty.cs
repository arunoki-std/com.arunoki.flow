namespace Arunoki.Flow
{
  public class BoolProperty<TEvent> : ValueProperty<bool, TEvent> where TEvent : struct, IValueEvent<bool>
  {
    public BoolProperty (bool defaultValue = false) : base (defaultValue)
    {
    }

    public static implicit operator bool (BoolProperty<TEvent> a) => a.Value;
  }
}