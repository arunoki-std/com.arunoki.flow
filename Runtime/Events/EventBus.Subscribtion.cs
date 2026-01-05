#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

using Arunoki.Collections;
using Arunoki.Flow.Misc;
using Arunoki.Flow.Utilities;

using System;
using System.Linq;

namespace Arunoki.Flow
{
  public partial class EventBus
  {
    [Obsolete ("Manual invocation is not desirable.")]
    public override void Add (Type key, EventChannel element) => base.Add (key, element);

    protected internal void Add (EventChannel channel)
    {
      base.Add (channel.GetEventType (), channel);
    }

    protected override void OnElementRemoved (EventChannel eventChannel)
    {
      base.OnElementRemoved (eventChannel);

      eventChannel.Clear ();
    }

    public void Subscribe (IHandler handler)
    {
      EventBusUtility.Subscribe (this, handler);
    }

    public void Subscribe (Type staticHandler)
    {
      EventBusUtility.Subscribe (this, staticHandler);
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
  }
}