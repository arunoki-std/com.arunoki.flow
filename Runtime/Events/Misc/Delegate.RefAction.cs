namespace Arunoki.Flow
{
  public delegate void RefAction<TEvent> (ref TEvent evt) where TEvent : struct, IEvent;

  public delegate void RefAction<in TTarget, TEvent> (TTarget target, ref TEvent evt) where TEvent : struct, IEvent;
}