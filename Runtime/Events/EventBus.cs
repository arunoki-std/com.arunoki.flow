using Arunoki.Collections;
using Arunoki.Flow.Utilities;
using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  /// Represents Key (event) && Element (event channel) collection.
  public partial class EventBus : Set<Type, EventChannel>, IResetable
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

    /// Reset each event channel if its <see cref="IResetable"/>.
    public virtual void Reset ()
    {
      foreach (var pair in Elements)
        if (pair.Element is IResetable resetable)
          resetable.Reset ();
    }
  }
}