namespace Arunoki.Flow
{
  public delegate void RefActionEvent<TEvent> (ref TEvent evt) where TEvent : struct, IEvent;
}