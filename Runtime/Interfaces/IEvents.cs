namespace Arunoki.Flow
{
  public interface IEvent
  {
    /// <summary>
    /// Context of the event.
    /// </summary>
    IContext Context { get; set; }
  }

  public interface IDomainEvent : IEvent
  {
    string Message { get; set; }
  }

  public interface IValueEvent<T> : IEvent
  {
    T Value { get; set; }

    T Previous { get; set; }
  }

  public interface IDataEvent<T> : IEvent
  {
    T Data { get; set; }
  }
}