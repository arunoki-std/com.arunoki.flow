namespace Arunoki.Flow
{
  public interface IStateBuilder<TEntity> where TEntity : class
  {
    void AddState<TState> () where TState : IState<TEntity>, new ();

    void InitRoot<TState> () where TState : IState<TEntity>, new ();
    void InitRouter<TRouter> () where TRouter : IStateRouter<TEntity>, new ();

    void ProduceAllFrom (object source);
    void ProduceStatesFrom (object source);
    void ProduceRoutersFrom (object source);
  }
}