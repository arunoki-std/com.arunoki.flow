namespace Arunoki.Flow
{
  public class Property<TEvent, TValue> : EventChannel<TEvent>, IProperty<TValue, TEvent>
    where TEvent : struct, IValueEvent<TValue>
  {
    public TValue Value { get; private set; }

    public TValue Previous { get; private set; }

    public bool IsReadable { get; private set; }

    public void SetReadable (bool readable) => IsReadable = readable;

    public virtual TValue Set (TValue value)
    {
      if (IsReadable) return Value;
      if (!Equals (value, Value))
      {
        Previous = Value;
        Value = value;

        Publish ();
      }

      return value;
    }

    /// Update values if needed and publish event anyway
    public virtual TValue Force (TValue value)
    {
      if (!Equals (value, Value))
      {
        Previous = Value;
        Value = value;
      }

      Publish ();
      return value;
    }

    /// Set values to default.
    public virtual void Reset (TValue @default = default)
    {
      Value = @default;
      Previous = @default;
    }

    /// Remove all subscribers.
    public override void Clear ()
    {
      base.Clear ();

      Reset ();
    }

    protected override TEvent GetEventInstance ()
    {
      return new TEvent { Context = this.Context, Value = this.Value, Previous = this.Previous };
    }
  }
}