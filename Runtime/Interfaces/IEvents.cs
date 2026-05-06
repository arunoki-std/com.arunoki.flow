namespace Arunoki.Flow
{
  public interface IEvent
  {
    IFlowContext Context { get; set; }
  }

  public interface IDomainEvent : IEvent
  {
  }

  public interface IValueEvent<T> : IEvent
  {
    T Current { get; set; }

    T Previous { get; set; }
  }

  public interface IDataEvent<T> : IEvent
  {
    T Data { get; set; }
  }
}