using Arunoki.Flow.Events;
using Arunoki.Flow.Events.Core;

namespace Arunoki.Flow.Utilities
{
  public static class CallbackUtility
  {
    public static void Activate (this Callback callback, EventBus eventBus)
    {
      eventBus.Channels [callback.EventType].Add (callback);
    }

    public static void Deactivate (this Callback callback, EventBus eventBus)
    {
      eventBus.Channels [callback.EventType].Remove (callback);
    }
  }
}