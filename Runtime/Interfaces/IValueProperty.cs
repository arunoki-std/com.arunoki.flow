namespace Arunoki.Flow
{
  public interface IValueProperty<TValue> : IEventChannel
  {
    TValue Set (TValue value);
  }

  public interface IValueProperty<TValue, TEvent> : IValueProperty<TValue>, IEventChannel<TEvent> where TEvent : IEvent
  {
  }
}