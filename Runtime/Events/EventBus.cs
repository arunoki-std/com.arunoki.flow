using Arunoki.Collections;
using Arunoki.Collections.Enumerators;
using Arunoki.Flow.Events.Core;
using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Events
{
  /// Represents Key (event) && Element (event channel) collection.
  public partial class EventBus : IResettable
  {
    public EventBus ()
    {
      Channels = new(new Container (this));
    }

    protected internal Set<Type, Channel> Channels { get; }

    /// Register reactive properties.
    public void RegisterSource (IContext context)
    {
      this.GetEventChannels (context);
    }

    /// Register reactive properties.
    public void RegisterSource (Type staticType)
    {
      this.GetEventChannels (staticType);
    }

    public void UnregisterSource (Type staticEventSource)
    {
      foreach ((int index, _, Channel channel) in Channels.WithIndex ())
        if (channel.Context is StaticContextWrapper wrapper && wrapper.IsConsumable (staticEventSource))
          Channels.RemoveAt (index);
    }

    public void UnregisterSource (IContext context)
    {
      foreach ((int index, _, Channel channel) in Channels.WithIndex ())
        if (context.Equals (channel.Context))
          Channels.RemoveAt (index);
    }

    bool IResettable.AutoReset () => true;

    /// Reset each event channel if its <see cref="IResettable"/> and its <see cref="IResettable.AutoReset"/> is on.
    public virtual void Reset ()
    {
      foreach (var channel in Channels)
        if (channel is IResettable resettable && resettable.AutoReset ())
          resettable.Reset ();
    }

    public virtual void ClearAll ()
    {
      Channels.Clear ();
    }

    public MutablePairValueEnumerator<Type, Channel> GetEnumerator () => Channels.GetEnumerator ();
  }
}