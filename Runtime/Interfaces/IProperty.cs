namespace Arunoki.Flow
{
  public interface IProperty<in TValue> : IEventChannel
  {
    bool Set (TValue value);
  }

  public interface IProperty<in TValue, TEvent> : IProperty<TValue>, IEventChannel<TEvent> where TEvent : IEvent
  {
  }
}