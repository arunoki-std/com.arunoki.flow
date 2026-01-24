using Arunoki.Collections;
using Arunoki.Flow.Events.Core;
using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Events
{
  /// Represents Key (event) && Element (event channel) collection.
  public partial class EventBus : Set<Type, Channel>, IResettable
  {
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
      foreach (var (index, _, channel) in WithIndex ())
        if (channel.Context is StaticContextWrapper wrapper && wrapper.IsConsumable (staticEventSource))
          RemoveAt (index);
    }

    public void UnregisterSource (IContext context)
    {
      foreach (var (index, _, channel) in WithIndex ())
        if (context.Equals (channel.Context))
          RemoveAt (index);
    }

    /// Reset each event channel if its <see cref="IResettable"/>.
    public virtual void Reset ()
    {
      foreach (var pair in Elements)
        if (pair.Element is IResettable resettable)
          resettable.Reset ();
    }
  }
}