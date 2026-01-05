using System;

namespace Arunoki.Flow
{
  public class DuplicateEventSubscription : Exception
  {
    public readonly Type Event;
    public readonly Type Handler;

    public DuplicateEventSubscription (Type eventType, object eventTarget)
      : base ($"Trying to subscribe '{Nameof (eventTarget)}' twice on event '{eventType}'.")
    {
      Event = eventType;
      Handler = eventTarget is Type type ? type : eventTarget.GetType ();
    }

    private static string Nameof (object target)
      => target is Type type ? type.Name : target.GetType ().Name;
  }
}