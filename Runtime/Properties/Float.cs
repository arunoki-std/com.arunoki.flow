namespace Arunoki.Flow
{
  public class Float<TEvent> : Property<TEvent, float> where TEvent : IValueEvent<float>, new ()
  {
    public static implicit operator int (Float<TEvent> a) => Globals.ToInt (a.Value);

    public static implicit operator uint (Float<TEvent> a) => Globals.ToUint (a.Value);

    public static implicit operator float (Float<TEvent> a) => a.Value;

    public static Float<TEvent> operator + (Float<TEvent> property, float b)
    {
      property.Set (property.Value + b);
      return property;
    }

    public static Float<TEvent> operator + (Float<TEvent> property, int b)
    {
      property.Set (property.Value + b);
      return property;
    }

    public static Float<TEvent> operator + (Float<TEvent> property, uint b)
    {
      property.Set (property.Value + b);
      return property;
    }

    public static Float<TEvent> operator - (Float<TEvent> property, float b)
    {
      property.Set (property.Value - b);
      return property;
    }

    public static Float<TEvent> operator - (Float<TEvent> property, int b)
    {
      property.Set (property.Value - b);
      return property;
    }

    public static Float<TEvent> operator - (Float<TEvent> property, uint b)
    {
      property.Set (property.Value - b);
      return property;
    }
  }
}