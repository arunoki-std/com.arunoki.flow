namespace Arunoki.Flow
{
  public interface IReadable<out TValue> : IEventChannel
  {
    TValue Value { get; }

    TValue Previous { get; }
  }

  public interface IReadable<out TValue, TEvent> : IReadable<TValue> where TEvent : IEvent
  {
  }
}