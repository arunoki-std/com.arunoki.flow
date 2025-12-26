namespace Arunoki.Flow
{
  public interface IProperty<TValue> : IReadable<TValue>
  {
    bool Set (TValue value);
  }

  public interface IProperty<TValue, TEvent> : IProperty<TValue>, IReadable<TValue, TEvent> where TEvent : IEvent
  {
  }
}