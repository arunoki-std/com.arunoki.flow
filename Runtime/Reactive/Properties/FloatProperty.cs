using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public class FloatProperty<TEvent> : ValueProperty<float, TEvent> where TEvent : struct, IValueEvent<float>
  {
    public FloatProperty (float defaultValue = 0.0f) : base (defaultValue)
    {
    }

    public void Add (float value) => Set (Value + value);
    public void Subtract (float value) => Set (Value - value);
    public void Multiply (float value) => Set (Value * value);
    public void Divide (float value) => Set (Value / value);

    public static implicit operator int (FloatProperty<TEvent> a) => Utils.ToInt (a.Value);

    public static implicit operator uint (FloatProperty<TEvent> a) => Utils.ToUint (a.Value);

    public static implicit operator float (FloatProperty<TEvent> a) => a.Value;

    public static FloatProperty<TEvent> operator + (FloatProperty<TEvent> property, float b)
    {
      property.Set (property.Value + b);
      return property;
    }

    public static FloatProperty<TEvent> operator + (FloatProperty<TEvent> property, int b)
    {
      property.Set (property.Value + b);
      return property;
    }

    public static FloatProperty<TEvent> operator + (FloatProperty<TEvent> property, uint b)
    {
      property.Set (property.Value + b);
      return property;
    }

    public static FloatProperty<TEvent> operator - (FloatProperty<TEvent> property, float b)
    {
      property.Set (property.Value - b);
      return property;
    }

    public static FloatProperty<TEvent> operator - (FloatProperty<TEvent> property, int b)
    {
      property.Set (property.Value - b);
      return property;
    }

    public static FloatProperty<TEvent> operator - (FloatProperty<TEvent> property, uint b)
    {
      property.Set (property.Value - b);
      return property;
    }
  }
}