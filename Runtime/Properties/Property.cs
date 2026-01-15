namespace Arunoki.Flow
{
  public class Property<TEvent, TValue> : Channel<TEvent>, IProperty<TValue, TEvent>, IResetable
    where TEvent : struct, IValueEvent<TValue>
  {
    private readonly TValue defaultValue;

    public Property (TValue defaultValue)
    {
      this.defaultValue = defaultValue;
    }

    public TValue Value { get; private set; }

    public TValue Previous { get; private set; }

    public virtual TValue Set (TValue value)
    {
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

    protected override TEvent GetEventInstance ()
    {
      return new TEvent { Context = this.Context, Value = this.Value, Previous = this.Previous };
    }
  }
}