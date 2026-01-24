using Arunoki.Flow.Misc;
using Arunoki.Flow.Utilities;

using System.Reflection;

namespace Arunoki.Flow
{
  public class Channel<TEvent> : EventChannel
    where TEvent : struct, IEvent
  {
    public event RefActionEvent<TEvent> OnEvent;

    public Channel () : base (typeof(TEvent))
    {
    }

    /// <exception cref="MultipleEventSubscriptionException"></exception>
    protected internal override Callback Subscribe (object target, MethodInfo [] methods)
    {
      if (Utils.IsDebug () && Any (callback => callback.IsConsumable (target)))
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

    /// Methods from <see cref="IHandler"/> will be invoked first and after them <see cref="OnEvent"/> delegates.
    protected internal virtual void Publish ()
    {
      TEvent evt = GetEventInstance ();

      Publish (ref evt);
    }

    protected virtual void Publish (ref TEvent evt)
    {
      for (var index = Elements.Count - 1; index >= 0; index--)
      {
        var callback = Elements [index] as Callback<TEvent>;
        if (callback.IsTargetReceivingEvents ())
          callback.Publish (ref evt);
      }

      if (OnEvent != null)
      {
        OnEvent (ref evt);
      }
    }

    protected virtual TEvent GetEventInstance ()
    {
      return new TEvent { Context = this.Context };
    }
  }
}