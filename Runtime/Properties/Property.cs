namespace Arunoki.Flow
{
  public class Property<TEvent, TValue> : EventChannel<TEvent>, IProperty<TValue, TEvent>
    where TEvent : struct, IValueEvent<TValue>
  {
    public TValue Value { get; private set; }

    public TValue Previous { get; private set; }

    public bool IsReadable { get; private set; }

    public void Readable (bool readable) => IsReadable = readable;

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

    public virtual void Reset (TValue @default = default)
    {
      Value = @default;
      Previous = @default;
      Readable (false);
    }

    public override void UnsubscribeAll ()
    {
      base.UnsubscribeAll ();

      Reset ();
    }

    protected override TEvent GetEventInstance ()
    {
      return new TEvent { Context = this.Context, Value = this.Value, Previous = this.Previous };
    }
  }
}