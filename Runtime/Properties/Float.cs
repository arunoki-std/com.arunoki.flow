using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public class Float<TEvent> : Property<TEvent, float> where TEvent : struct, IValueEvent<float>
  {
    public void Add (float value) => Set (Value + value);
    public void Subtract (float value) => Set (Value - value);
    public void Multiply (float value) => Set (Value * value);
    public void Divide (float value) => Set (Value / value);

    public static implicit operator int (Float<TEvent> a) => Utils.ToInt (a.Value);

    public static implicit operator uint (Float<TEvent> a) => Utils.ToUint (a.Value);

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