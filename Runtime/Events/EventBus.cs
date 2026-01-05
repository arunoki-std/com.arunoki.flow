using Arunoki.Collections;
using Arunoki.Flow.Utilities;
using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  /// Represents Key (event) && Element (event channel) collection.
  public partial class EventBus : Set<Type, EventChannel>
  {
    /// Register reactive properties.
    public void AddEventSource (IContext context)
    {
      this.GetReactiveProperties (context);
    }

    /// Register reactive properties.
    public void AddEventSource (Type staticType)
    {
      this.GetReactiveProperties (staticType);
    }

    public void RemoveEvents (Type staticEventSource)
    {
      foreach (var (index, _, channel) in WithIndex ())
        if (channel.Context is StaticContextWrapper wrapper && wrapper.IsConsumable (staticEventSource))
          RemoveAt (index);
    }

    public void RemoveEvents (IContext context)
    {
      foreach (var (index, _, channel) in WithIndex ())
        if (context.Equals (channel.Context))
          RemoveAt (index);
    }
  }
}