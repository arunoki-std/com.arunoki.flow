using Arunoki.Flow.Events.Core;
using Arunoki.Flow.Utilities;

using System.Reflection;

namespace Arunoki.Flow.Events
{
  public class Channel<TEvent> : Channel
    where TEvent : struct, IEvent
  {
    public event RefActionEvent<TEvent> OnEvent;

    public Channel () : base (typeof(TEvent))
    {
    }

    /// <exception cref="MultipleEventSubscriptionException"></exception>
    protected internal override Callback Subscribe (object target, MethodInfo [] methods)
    {
      if (Utils.IsDebug () && Callbacks.Find (cb => cb.IsConsumable (target)) != null)
        throw new MultipleEventSubscriptionException (GetEventType (), target);

      var callback = new Callback<TEvent> (target, GetEventType (), methods);
      Add (callback);
      return callback;
    }

    /// Remove all subscribers.
    public override void Clear ()
    {
      base.Clear ();

      OnEvent = null;
    }

    /// Publish event.
    /// Methods from <see cref="IHandler"/> will be invoked first and after them <see cref="OnEvent"/> delegates.
    protected internal virtual void Publish ()
    {
      var evt = GetEventInstance ();

      Publish (ref evt);
    }

    /// Publish event.
    /// Methods from <see cref="IHandler"/> will be invoked first and after them <see cref="OnEvent"/> delegates.
    protected virtual void Publish (ref TEvent evt)
    {
      for (var index = Callbacks.Count - 1; index >= 0; index--)
      {
        var callback = Callbacks [index];

        if (!callback.CanReceiveEvents ())
          continue;

        (callback as Callback<TEvent>).Publish (ref evt);
      }

      OnEvent?.Invoke (ref evt);
    }

    protected virtual TEvent GetEventInstance ()
    {
      return new TEvent { Context = this.Context };
    }
  }
}