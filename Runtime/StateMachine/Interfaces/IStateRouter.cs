namespace Arunoki.Flow
{
  public interface IStateRouter<TEntity> : IHandler, IInitializable, IStartable, IResettable
    where TEntity : class
  {
    TEntity Entity { get; set; }
    StateMachine<TEntity> Machine { get; set; }
  }
}