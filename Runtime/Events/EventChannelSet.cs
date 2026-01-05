using Arunoki.Collections;

using System;

namespace Arunoki.Flow.Misc
{
  public class EventChannelSet : Set<Type, EventChannel>
  {
    public void Add (EventChannel channel)
    {
      Add (channel.GetEventType (), channel);
    }

    protected override void OnElementRemoved (EventChannel element)
    {
      base.OnElementRemoved (element);

      element.UnsubscribeAll ();
    }

    public void RemoveBy (IContext context)
    {
      RemoveWhere (channel => context.Equals (channel.Context));
    }

    public virtual void Unsubscribe (IHandler handler)
    {
      foreach (var pair in Elements)
      foreach ((int index, EventsHandler callback) in pair.Value.Handlers.WithIndex ())
        if (callback.IsTarget (handler))
          pair.Value.Handlers.RemoveAt (index);
    }

    public virtual void UnsubscribeAll ()
    {
      Elements.ForEach (pair => pair.Value.UnsubscribeAll ());
    }
  }
}