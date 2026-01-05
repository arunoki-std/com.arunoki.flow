#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

using System;

namespace Arunoki.Flow
{
  /// One type of event per data
  public class EventChannel<TEvent, TData> : EventChannel<TEvent>
    where TEvent : struct, IDataEvent<TData>
    where TData : struct
  {
    public virtual void Publish (ref TData data)
    {
      var evt = GetEventInstance ();
      evt.Data = data;

      Publish (ref evt);
    }

    [Obsolete ("Use Publish (ref TData) instead")]
    public override void Publish ()
    {
      base.Publish ();
    }
  }
}