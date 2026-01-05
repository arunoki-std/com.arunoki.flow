using Arunoki.Flow.Utilities;
using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  public class EventBus : IDisposable, IBuilder
  {
    internal readonly EventChannelSet Channels;

    public EventBus () : this (new EventChannelSet ()) { }

    internal EventBus (EventChannelSet set)
    {
      Channels = set ?? new EventChannelSet ();
    }

    public void Subscribe (IHandler handler)
    {
      Channels.Subscribe (handler, (target, methods)
        => new EventsHandler (target, methods));
    }

    public void Unsubscribe (IHandler handler)
    {
      Channels.Unsubscribe (handler);
    }

    public void Subscribe (Type staticSubscriber)
    {
      Channels.Subscribe (staticSubscriber, (target, methods)
        => new ProxyTypeEventsHandler ((Type) target, methods));
    }

    public void Unsubscribe (Type staticSubscriber)
    {
      foreach (var channel in Channels)
      foreach (var handler in channel.Handlers.Where (eventsHandler => eventsHandler.IsTarget (staticSubscriber)))
        channel.Unsubscribe (handler);
    }

    public void UnsubscribeAll ()
    {
      Channels.UnsubscribeAll ();
    }

    public void Register (IContext context)
    {
      Channels.RegisterEvents (context);
    }

    /// <summary>
    /// Register reactive properties
    /// </summary>
    /// <param name="staticType"></param>
    public void Register (Type staticType)
    {
      Channels.RegisterEvents (staticType);
    }

    public void Clear (IContext context)
    {
      Channels.RemoveBy (context);
    }

    public void Clear (Type staticEventSource)
    {
      Channels.RemoveWhere (eventChannel =>
        eventChannel.Context is ProxyTypeContext wrapper && wrapper.Type == staticEventSource);
    }

    public virtual void Dispose ()
    {
      Channels.Clear ();
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

    public static bool IsConsumable (Type itemType) => itemType.IsAbstract && itemType.IsSealed;
  }
}