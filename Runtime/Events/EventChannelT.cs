using System;

namespace Arunoki.Flow
{
  public class EventChannel<TEvent> : EventChannel
    where TEvent : IDomainEvent, new ()
  {
    public event EventReceiver<TEvent> OnEvent = delegate { };

    public EventChannel () : base (typeof(TEvent))
    {
    }

    public override void UnsubscribeAll ()
    {
      base.UnsubscribeAll ();

      OnEvent = delegate { };
    }

    public void Call ()
    {
      var evt = (TEvent) Activator.CreateInstance (GetEventType (), Context);

      base.Call (evt);
      OnEvent (evt);
    }

    protected internal sealed override void Call (object @event)
    {
      base.Call (@event);

      OnEvent ((TEvent) @event);
    }
  }
}