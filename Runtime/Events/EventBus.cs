using Arunoki.Collections;
using Arunoki.Flow.Utilities;
using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  /// Represents Key (event) && Element (event channel) collection.
  public partial class EventBus : Set<Type, EventChannel>, IResettable
  {
    /// Register reactive properties.
    public void RegisterSource (IContext context)
    {
      this.GetReactiveProperties (context);
    }

    /// Register reactive properties.
    public void RegisterSource (Type staticType)
    {
      this.GetReactiveProperties (staticType);
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