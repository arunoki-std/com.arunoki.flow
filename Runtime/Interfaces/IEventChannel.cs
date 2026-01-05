using System;

namespace Arunoki.Flow
{
  public interface IEventChannel
  {
    Type GetEventType ();
  }

  public interface IEventChannel<TEvent> : IEventChannel where TEvent : IEvent
  {
  }
}