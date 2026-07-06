using System;
using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
    public interface IEventChannel
    {
        Type GetEventType();
    }

    public interface IEventChannel<TEvent> : IEventChannel
        where TEvent : IEvent { }

    public interface IObservableEventChannel<out TValue>
    {
        event Action<Channel, TValue> OnUpdated;
    }
}
