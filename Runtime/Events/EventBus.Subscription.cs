#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

using Arunoki.Flow.Events.Core;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Events
{
  public partial class EventBus
  {
    public List<Callback> Subscribe (IHandler handler)
    {
      return EventBusUtility.Subscribe (this, handler);
    }

    public List<Callback> Subscribe (Type staticHandler)
    {
      return EventBusUtility.Subscribe (this, staticHandler);
    }

    public void Unsubscribe (Type staticHandler) => Unsubscribe (staticHandler as object);

    public void Unsubscribe (IHandler handler) => Unsubscribe (handler as object);

    protected virtual void Unsubscribe (object handler)
    {
      foreach (var pair in Elements)
      foreach ((int index, Callback callback) in pair.Element.WithIndex ())
        if (callback.IsConsumable (handler))
          pair.Element.RemoveAt (index);
    }

    public virtual void UnsubscribeAll ()
    {
      Elements.ForEach (pair => pair.Element.Clear ());
    }

    protected internal void Add (Channel channel)
    {
      base.Add (channel.GetEventType (), channel);
    }

    [Obsolete ("Manual invocation is not desirable.")]
    public sealed override void Add (Type eventType, Channel channel) => Add (channel);

    protected override void OnElementRemoved (Channel channel)
    {
      base.OnElementRemoved (channel);

      channel.Clear ();
    }
  }
}