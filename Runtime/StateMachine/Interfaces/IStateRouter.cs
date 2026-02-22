namespace Arunoki.Flow
{
  public interface IStateRouter<TEntity> : IHandler
    where TEntity : class
  {
    TEntity Entity { get; set; }
    StateMachine<TEntity> Machine { get; set; }
  }
}