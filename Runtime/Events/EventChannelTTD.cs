using System;

namespace Arunoki.Flow
{
  /// <summary>
  /// One type of event per data
  /// </summary>
  /// <typeparam name="TEvent"></typeparam>
  /// <typeparam name="TData"></typeparam>
  public class EventChannel<TEvent, TData> : EventChannel where TEvent : IDataEvent<TData>, new ()
  {
    public event EventReceiver<TEvent> OnEvent = delegate { };

    public EventChannel () : base (typeof(TEvent))
    {
    }

    public void Send (TData data)
    {
      var evt = (TEvent) Activator.CreateInstance (GetEventType (), Context, data);

      base.Call (evt);
      OnEvent.Invoke (evt);
    }

    protected internal sealed override void Call (object message)
    {
      base.Call (message);

      OnEvent.Invoke ((TEvent) message);
    }

    public override void UnsubscribeAll ()
    {
      base.UnsubscribeAll ();

      OnEvent = delegate { };
    }
  }
}