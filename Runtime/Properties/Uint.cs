using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public class Uint<TEvent> : Property<TEvent, uint> where TEvent : IValueEvent<uint>, new ()
  {
    public void Add (uint value) => Set (Value + value);
    public void Subtract (uint value) => Set (Value - value);
    public void Multiply (uint value) => Set (Value * value);
    public void Divide (uint value) => Set (Value / value);

    public static implicit operator int (Uint<TEvent> a) => (int) a.Value;

    public static implicit operator uint (Uint<TEvent> a) => a.Value;

    public static implicit operator float (Uint<TEvent> a) => a.Value;

    public static Uint<TEvent> operator + (Uint<TEvent> property, float b)
    {
      property.Set (property.Value + Utils.ToUint (b));
      return property;
    }

    public static Uint<TEvent> operator + (Uint<TEvent> property, int b)
    {
      property.Set (property.Value + (uint) b);
      return property;
    }

    public static Uint<TEvent> operator + (Uint<TEvent> property, uint b)
    {
      property.Set (property.Value + b);
      return property;
    }

    public static Uint<TEvent> operator - (Uint<TEvent> property, float b)
    {
      property.Set (property.Value - Utils.ToUint (b));
      return property;
    }

    public static Uint<TEvent> operator - (Uint<TEvent> property, int b)
    {
      property.Set (property.Value - (uint) b);
      return property;
    }

    public static Uint<TEvent> operator - (Uint<TEvent> property, uint b)
    {
      property.Set (property.Value - b);
      return property;
    }
  }
}