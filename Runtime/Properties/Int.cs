using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public class Int<TEvent> : Property<TEvent, int> where TEvent : IValueEvent<int>, new ()
  {
    public void Add (int value) => Set (Value + value);
    public void Subtract (int value) => Set (Value - value);
    public void Multiply (int value) => Set (Value * value);
    public void Divide (int value) => Set (Value / value);

    public static implicit operator int (Int<TEvent> a) => a.Value;

    public static implicit operator uint (Int<TEvent> a) => (uint) a.Value;

    public static implicit operator float (Int<TEvent> a) => a.Value;

    public static Int<TEvent> operator + (Int<TEvent> property, float b)
    {
      property.Set (property.Value + Utils.ToInt (b));
      return property;
    }

    public static Int<TEvent> operator + (Int<TEvent> property, int b)
    {
      property.Set (property.Value + b);
      return property;
    }

    public static Int<TEvent> operator + (Int<TEvent> property, uint b)
    {
      property.Set (property.Value + (int) b);
      return property;
    }

    public static Int<TEvent> operator - (Int<TEvent> property, float b)
    {
      property.Set (property.Value - Utils.ToInt (b));
      return property;
    }

    public static Int<TEvent> operator - (Int<TEvent> property, int b)
    {
      property.Set (property.Value - b);
      return property;
    }

    public static Int<TEvent> operator - (Int<TEvent> property, uint b)
    {
      property.Set (property.Value - (int) b);
      return property;
    }
  }
}