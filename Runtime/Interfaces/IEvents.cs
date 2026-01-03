namespace Arunoki.Flow
{
  public interface IEvent
  {
    /// <summary>
    /// Context of the event.
    /// </summary>
    IContext Context { get; }
  }

  public interface IDomainEvent : IEvent
  {
    bool GetMessage (out string message);
  }

  public interface IValueEvent<out T> : IEvent
  {
    T Value { get; }

    T Previous { get; }
  }

  public interface IDataEvent<out T> : IEvent
  {
    T Data { get; }
  }
}