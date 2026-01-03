using Arunoki.Flow.Utils;
using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  public class EventBus : IDisposable, IBuilder
  {
    internal readonly EventChannelSet EventChannels;

    public EventBus () : this (new EventChannelSet ()) { }

    internal EventBus (EventChannelSet set)
    {
      EventChannels = set ?? new EventChannelSet ();
    }

    public void Subscribe (IHandler handler)
    {
      EventChannels.Subscribe (handler, (target, methods)
        => new EventsHandler (target, methods));
    }

    public void Unsubscribe (IHandler handler)
    {
      EventChannels.Unsubscribe (handler);
    }

    public void Subscribe (Type staticSubscriber)
    {
      EventChannels.Subscribe (staticSubscriber, (target, methods)
        => new ProxyTypeEventsHandler ((Type) target, methods));
    }

    public void Unsubscribe (Type staticSubscriber)
    {
      EventChannels.Unsubscribe (staticSubscriber);
    }

    public void UnsubscribeAll ()
    {
      EventChannels.UnsubscribeAll ();
    }

    public void Register (IContext context)
    {
      EventChannels.RegisterEvents (context);
    }

    /// <summary>
    /// Register reactive properties
    /// </summary>
    /// <param name="staticType"></param>
    public void Register (Type staticType)
    {
      EventChannels.RegisterEvents (staticType);
    }

    public void Clear (IContext context)
    {
      EventChannels.RemoveBy (context);
    }

    public void Clear (Type staticEventSource)
    {
      EventChannels.RemoveWhere (eventChannel =>
        eventChannel.Context is ProxyTypeContext wrapper && wrapper.Type == staticEventSource);
    }

    public virtual void Dispose ()
    {
      EventChannels.Dispose ();
    }

    void IBuilder.Build (object element)
    {
      if (element is IContext ec) Register (ec);
      if (element is IHandler er) Subscribe (er);
      if (element is Type t && IsConsumable (t))
      {
        Register (t);
        Subscribe (t);
      }
    }

    public bool IsConsumable (object element) => element is IContext;

    public bool IsConsumable (Type itemType) => itemType.IsAbstract && itemType.IsSealed;
  }
}