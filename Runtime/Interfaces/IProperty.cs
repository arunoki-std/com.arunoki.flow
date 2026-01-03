namespace Arunoki.Flow
{
  public interface IProperty<TValue> : IEventChannel
  {
    TValue Set (TValue value);
  }

  public interface IProperty<TValue, TEvent> : IProperty<TValue>, IEventChannel<TEvent> where TEvent : IEvent
  {
  }
}