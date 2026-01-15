namespace Arunoki.Flow
{
  public class Trigger<TEvent> : Channel<TEvent>, IResetable where TEvent : struct, IDomainEvent
  {
    public bool IsTriggered { get; private set; }

    public void Activate ()
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