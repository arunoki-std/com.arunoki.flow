namespace Arunoki.Flow
{
  public delegate void EventReceiver<TEvent> (ref TEvent evt) where TEvent : struct, IEvent;
}