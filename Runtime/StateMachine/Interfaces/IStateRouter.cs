namespace Arunoki.Flow
{
  public interface IStateRouter<TEntity> : IHandler, IStartable, IResettable
    where TEntity : class
  {
    TEntity Entity { get; set; }
    StateMachine<TEntity> Machine { get; set; }
  }
}