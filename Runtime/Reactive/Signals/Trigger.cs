using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
  public class Trigger<TEvent> : Channel<TEvent>, IResettable where TEvent : struct, IDomainEvent
  {
    private readonly bool autoReset;

    public Trigger (bool autoReset = true)
    {
      this.autoReset = autoReset;
    }

    public bool IsTriggered { get; private set; }

    public void Set ()
    {
      if (!IsTriggered)
      {
        IsTriggered = true;

        Publish ();
      }
    }

    public bool AutoReset () => autoReset;

    public void Reset ()
    {
      IsTriggered = false;
    }

    public static implicit operator bool (Trigger<TEvent> a) => a.IsTriggered;
  }
}