namespace Arunoki.Flow
{
  public delegate void EventReceiver<in TEvent> (TEvent evt) where TEvent : IEvent;
}