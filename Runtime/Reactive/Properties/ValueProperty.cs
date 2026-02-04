using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
  public class ValueProperty<TValue, TEvent> : Channel<TEvent>, IValueProperty<TValue, TEvent>, IResettable
    where TEvent : struct, IValueEvent<TValue>
  {
    private readonly TValue defaultValue;
    private readonly bool autoReset;

    public ValueProperty (bool autoReset = true)
      : this (default, autoReset)
    {
    }

    public ValueProperty (TValue defaultValue, bool autoReset = true)
    {
      this.defaultValue = defaultValue;
      this.autoReset = autoReset;
    }

    public TValue Value { get; private set; }

    public TValue Previous { get; private set; }

    public bool AutoReset () => autoReset;

    protected virtual bool TryChange (ref TValue value)
    {
      var current = Value;
      if (!Equals (ref value, ref current))
      {
        Previous = current;
        Value = value;

        return true;
      }

      return false;
    }

    public virtual TValue Set (TValue value)
    {
      if (TryChange (ref value)) Publish ();
      return value;
    }

    /// Update values if needed and publish event anyway
    public virtual TValue Force (TValue value)
    {
      TryChange (ref value);
      Publish ();
      return value;
    }

    /// Set values to default.
    public virtual void Reset ()
    {
      Value = defaultValue;
      Previous = defaultValue;
    }

    /// Remove all subscribers.
    public override void Clear ()
    {
      base.Clear ();

      Reset ();
    }

    protected virtual bool Equals (ref TValue a, ref TValue b) => ReferenceEquals (a, b);

    protected override TEvent GetEventInstance ()
    {
      return new TEvent { Context = this.Context, Value = this.Value, Previous = this.Previous };
    }
  }
}