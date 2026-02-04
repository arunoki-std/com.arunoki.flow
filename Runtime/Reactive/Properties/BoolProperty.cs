namespace Arunoki.Flow
{
  public class BoolProperty<TEvent> : ValueProperty<bool, TEvent> where TEvent : struct, IValueEvent<bool>
  {
    public BoolProperty (bool autoReset = true)
      : base (false, autoReset)
    {
    }

    public BoolProperty (bool defaultValue = false, bool autoReset = true)
      : base (defaultValue, autoReset)
    {
    }

    public static implicit operator bool (BoolProperty<TEvent> a) => a.Value;
  }
}