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

    public bool Set (TValue value)
    {
      if (!Equals (value, Value))
      {
        Previous = Value;
        Value = value;

        Call ();

        return true;
      }

      return false;
    }

    public void Force (TValue value)
    {
      if (!Set (value))
      {
        Call ();
      }
    }

    protected void Call ()
    {
      var evt = Activator.CreateInstance (GetEventType (), Context, Value, Previous);
      base.Call (ref evt);

      var domainEvent = (TEvent) evt;
      OnEvent?.Invoke (ref domainEvent);
    }

    protected internal sealed override void Call (ref object evt)
    {
      base.Call (ref evt);

      var domainEvent = (TEvent) evt;
      OnEvent?.Invoke (ref domainEvent);
    }

    public virtual void Reset (TValue @default = default)
    {
      Value = @default;
      Previous = @default;
    }

    public override void UnsubscribeAll ()
    {
      base.UnsubscribeAll ();

      OnEvent = null;
      Reset ();
    }
  }
}