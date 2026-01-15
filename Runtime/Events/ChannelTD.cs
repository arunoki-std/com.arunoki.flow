#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

using System;

namespace Arunoki.Flow
{
  /// One type of event per data.
  public class Channel<TEvent, TData> : Channel<TEvent>
    where TEvent : struct, IDataEvent<TData>
  {
    public virtual void Publish (ref TData data)
    {
      var evt = GetEventInstance ();
      evt.Data = data;

      Publish (ref evt);
    }

    [Obsolete ("Use Publish (ref TData) instead.")]
    protected internal override void Publish ()
    {
      base.Publish ();
    }
  }
}