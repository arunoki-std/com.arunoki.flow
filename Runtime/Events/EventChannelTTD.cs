using System;

namespace Arunoki.Flow
{
  /// One type of event per data
  public class EventChannel<TEvent, TData> : EventChannel where TEvent : IDataEvent<TData>, new ()
  {
    public event EventReceiver<TEvent> OnEvent;

    public EventChannel () : base (typeof(TEvent))
    {
    }

    public void Send (TData data)
    {
      var evt = Activator.CreateInstance (GetEventType (), Context, data);
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

    public override void UnsubscribeAll ()
    {
      base.UnsubscribeAll ();

      OnEvent = null;
    }
  }
}