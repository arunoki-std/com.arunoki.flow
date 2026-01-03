using System;

namespace Arunoki.Flow
{
  public class Property<TEvent, TValue> : EventChannel, IProperty<TValue, TEvent>
    where TEvent : IValueEvent<TValue>, new ()
  {
    public Property () : base (typeof(TEvent))
    {
    }

    public event EventReceiver<TEvent> OnEvent;

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

        Call ();
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

      Call ();
      return value;
    }

    protected void Call ()
    {
      var evt = Activator.CreateInstance (GetEventType (), Context, Value, Previous);
      base.Call (ref evt);

      if (OnEvent != null)
      {
        var domainEvent = (TEvent) evt;
        OnEvent (ref domainEvent);
      }
    }

    protected internal sealed override void Call (ref object evt)
    {
      base.Call (ref evt);

      if (OnEvent != null)
      {
        var domainEvent = (TEvent) evt;
        OnEvent (ref domainEvent);
      }
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

      OnEvent = null;
      Reset ();
    }
  }
}