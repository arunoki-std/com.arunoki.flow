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

    public void Subscribe (IEventsHandler eventsHandler)
    {
      EventChannels.Subscribe (eventsHandler, (target, methods)
        => new EventsHandler (target, methods));
    }

    public void Unsubscribe (IEventsHandler eventsHandler)
    {
      EventChannels.Unsubscribe (eventsHandler);
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

    public void Register (IEventsContext context)
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

    public void Clear (IEventsContext context)
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

    void IBuilder.Build (object item)
    {
      if (item is IEventsContext ec) Register (ec);
      if (item is IEventsHandler er) Subscribe (er);
      if (item is Type t)
      {
        Register (t);
        Subscribe (t);
      }
    }

    public bool IsConsumable (object item) => item is IEventsContext;

    public bool IsConsumable (Type itemType) => itemType.IsAbstract && itemType.IsSealed;
  }
}