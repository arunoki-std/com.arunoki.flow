namespace Arunoki.Flow
{
  public class EventChannel<TEvent> : EventChannel
    where TEvent : struct, IEvent
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

    public void Publish ()
    {
      var evt = GetEventInstance ();
      base.Publish (ref evt);

      if (OnEvent != null)
      {
        OnEvent (ref evt);
      }
    }

    protected virtual TEvent GetEventInstance ()
    {
      return new TEvent { Context = this.Context };
    }
  }
}