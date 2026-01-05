using Arunoki.Flow.Misc;
using Arunoki.Flow.Utilities;

using System;

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

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

      eventChannel.UnsubscribeAll ();
    }

    public void Subscribe (IHandler handler)
    {
      this.Subscribe (handler, (target, methods)
        => new Callback (target, methods));
    }

    public void Subscribe (Type staticHandler)
    {
      this.Subscribe (staticHandler, (target, methods)
        => new StaticCallbackWrapper ((Type) target, methods));
    }

    public void Unsubscribe (Type staticHandler) => Unsubscribe (staticHandler as object);

    public void Unsubscribe (IHandler handler) => Unsubscribe (handler as object);

    protected virtual void Unsubscribe (object handler)
    {
      foreach (var pair in Elements)
      foreach ((int index, Callback callback) in pair.Element.Callbacks.WithIndex ())
        if (callback.IsTarget (handler))
          pair.Element.Callbacks.RemoveAt (index);
    }

    public virtual void UnsubscribeAll ()
    {
      Elements.ForEach (pair => pair.Element.UnsubscribeAll ());
    }
  }
}