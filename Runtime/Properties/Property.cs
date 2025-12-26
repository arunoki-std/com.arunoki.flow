using System;

namespace Arunoki.Flow
{
  public class Property<TEvent, TValue> : EventChannel, IProperty<TValue>
    where TEvent : IValueEvent<TValue>, new ()
  {
    public Property () : base (typeof(TEvent))
    {
    }

    public event EventReceiver<TEvent> OnEvent = delegate { };

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
      var evt = NewEvent ();

      base.Call (evt);
      OnEvent.Invoke (evt);
    }

    protected internal sealed override void Call (object message)
    {
      base.Call (message);

      OnEvent.Invoke ((TEvent) message);
    }

    public virtual void Reset (TValue @default = default)
    {
      Value = @default;
      Previous = @default;
    }

    public override void UnsubscribeAll ()
    {
      base.UnsubscribeAll ();

      Reset ();
    }

    protected virtual TEvent NewEvent ()
    {
      return (TEvent) Activator.CreateInstance (GetEventType (), Context, Value, Previous);
    }
  }
}