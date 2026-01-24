using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public class IntProperty<TEvent> : ValueProperty<int, TEvent> where TEvent : struct, IValueEvent<int>
  {
    public IntProperty (int defaultValue = 0) : base (defaultValue)
    {
    }

    public void Add (int value) => Set (Value + value);
    public void Subtract (int value) => Set (Value - value);
    public void Multiply (int value) => Set (Value * value);
    public void Divide (int value) => Set (Value / value);

    public static implicit operator int (IntProperty<TEvent> a) => a.Value;

    public static implicit operator uint (IntProperty<TEvent> a) => (uint) a.Value;

    public static implicit operator float (IntProperty<TEvent> a) => a.Value;

    public static IntProperty<TEvent> operator + (IntProperty<TEvent> property, float b)
    {
      property.Set (property.Value + Utils.ToInt (b));
      return property;
    }

    public static IntProperty<TEvent> operator + (IntProperty<TEvent> property, int b)
    {
      property.Set (property.Value + b);
      return property;
    }

    public static IntProperty<TEvent> operator + (IntProperty<TEvent> property, uint b)
    {
      property.Set (property.Value + (int) b);
      return property;
    }

    public static IntProperty<TEvent> operator - (IntProperty<TEvent> property, float b)
    {
      property.Set (property.Value - Utils.ToInt (b));
      return property;
    }

    public static IntProperty<TEvent> operator - (IntProperty<TEvent> property, int b)
    {
      property.Set (property.Value - b);
      return property;
    }

    public static IntProperty<TEvent> operator - (IntProperty<TEvent> property, uint b)
    {
      property.Set (property.Value - (int) b);
      return property;
    }
  }
}