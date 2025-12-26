using System;

namespace Arunoki.Flow
{
  public interface IEventChannel
  {
  }

  public interface IEventChannel<TEvent> : IEventChannel where TEvent : IEvent
  {
    Type GetEventType ();
  }
}