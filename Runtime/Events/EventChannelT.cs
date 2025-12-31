using System;

namespace Arunoki.Flow
{
  public class EventChannel<TEvent> : EventChannel
    where TEvent : IDomainEvent, new ()
  {
    public event EventReceiver<TEvent> OnEvent;

    public EventChannel () : base (typeof(TEvent))
    {
    }

    public override void UnsubscribeAll ()
    {
      base.UnsubscribeAll ();

      OnEvent = null;
    }

    public void Call ()
    {
      var evt = Activator.CreateInstance (GetEventType (), Context);
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
  }
}