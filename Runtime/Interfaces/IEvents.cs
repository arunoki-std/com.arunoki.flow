namespace Arunoki.Flow
{
  public interface IEvent
  {
    IContext Context { get; set; }
  }

  public interface IDomainEvent : IEvent
  {
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