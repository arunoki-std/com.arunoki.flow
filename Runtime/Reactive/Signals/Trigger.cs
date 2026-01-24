using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
  public class Trigger<TEvent> : Channel<TEvent>, IResettable where TEvent : struct, IDomainEvent
  {
    public bool IsTriggered { get; private set; }

    public void Fire ()
    {
      if (!IsTriggered)
      {
        IsTriggered = true;

        Publish ();
      }
    }

    public void Reset ()
    {
      IsTriggered = false;
    }
  }
}