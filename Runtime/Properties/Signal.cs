namespace Arunoki.Flow
{
  public class Signal<TEvent> : Channel<TEvent> where TEvent : struct, IDomainEvent
  {
    /// Call event.
    /// Methods from <see cref="IHandler"/>'s will be invoked first and after them event delegates.
    public void Call ()
    {
      Publish ();
    }
  }
}