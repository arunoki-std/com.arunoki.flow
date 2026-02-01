using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public class UintProperty<TEvent> : ValueProperty<uint, TEvent> where TEvent : struct, IValueEvent<uint>
  {
    public UintProperty (uint defaultValue = 0, bool autoReset = true)
      : base (defaultValue, autoReset)
    {
    }

    public void Add (uint value) => Set (Value + value);
    public void Subtract (uint value) => Set (Value - value);
    public void Multiply (uint value) => Set (Value * value);
    public void Divide (uint value) => Set (Value / value);

    public static implicit operator int (UintProperty<TEvent> a) => (int) a.Value;

    public static implicit operator uint (UintProperty<TEvent> a) => a.Value;

    public static implicit operator float (UintProperty<TEvent> a) => a.Value;

    public static UintProperty<TEvent> operator + (UintProperty<TEvent> property, float b)
    {
      property.Set (property.Value + Utils.ToUint (b));
      return property;
    }

    public static UintProperty<TEvent> operator + (UintProperty<TEvent> property, int b)
    {
      property.Set (property.Value + (uint) b);
      return property;
    }

    public static UintProperty<TEvent> operator + (UintProperty<TEvent> property, uint b)
    {
      property.Set (property.Value + b);
      return property;
    }

    public static UintProperty<TEvent> operator - (UintProperty<TEvent> property, float b)
    {
      property.Set (property.Value - Utils.ToUint (b));
      return property;
    }

    public static UintProperty<TEvent> operator - (UintProperty<TEvent> property, int b)
    {
      property.Set (property.Value - (uint) b);
      return property;
    }

    public static UintProperty<TEvent> operator - (UintProperty<TEvent> property, uint b)
    {
      property.Set (property.Value - b);
      return property;
    }
  }
}