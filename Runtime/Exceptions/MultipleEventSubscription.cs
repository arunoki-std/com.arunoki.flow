using System;

namespace Arunoki.Flow
{
  public class MultipleEventSubscription : Exception
  {
    public readonly Type Event;
    public readonly Type Handler;

    public MultipleEventSubscription (Type eventType, object eventTarget)
      : base ($"Trying to subscribe '{Nameof (eventTarget)}' multiple times on event '{eventType}'.")
    {
      Event = eventType;
      Handler = eventTarget is Type type ? type : eventTarget.GetType ();
    }

    private static string Nameof (object target)
      => target is Type type ? type.Name : target.GetType ().Name;
  }
}