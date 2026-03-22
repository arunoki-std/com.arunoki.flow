namespace Arunoki.Flow
{
  public class BoolProperty<TEvent> : ValueProperty<bool, TEvent> where TEvent : struct, IValueEvent<bool>
  {
    public BoolProperty (bool autoReset)
      : base (false, autoReset)
    {
    }

    public BoolProperty (bool defaultValue, bool autoReset = false)
      : base (defaultValue, autoReset)
    {
    }

    public BoolProperty () : base (false, false)
    {
    }

    public static implicit operator bool (BoolProperty<TEvent> a) => a.Value;
  }
}