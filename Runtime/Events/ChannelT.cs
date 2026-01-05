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

    /// <exception cref="DuplicateEventSubscription"></exception>
    protected internal override void Subscribe (object target, MethodInfo [] methods)
    {
      if (Utils.IsDebug () && Any (callback => callback.IsConsumable (target)))
        throw new DuplicateEventSubscription (GetEventType (), target);

      Add (new Callback<TEvent> (target, methods));
    }

    /// Remove all subscribers.
    public override void Clear ()
    {
      base.Clear ();

      OnEvent = null;
    }

    /// Methods from <see cref="IHandler"/> will be invoked first and after them <see cref="OnEvent"/> delegates.
    public virtual void Publish ()
    {
      TEvent evt = GetEventInstance ();

      Publish (ref evt);
    }

    protected virtual void Publish (ref TEvent evt)
    {
      for (var index = Elements.Count - 1; index >= 0; index--)
        (Elements [index] as Callback<TEvent>).Publish (ref evt);

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