namespace Arunoki.Flow
{
  public interface IFsmBuilder<TEntity> where TEntity : class
  {
    void AddState<TState> () where TState : IState<TEntity>, new ();
    void AddRouter<TRouter> () where TRouter : IStateRouter<TEntity>, new ();
    void GetStatesFrom (object source);
    void GetRoutersFrom (object source);
    void GetAllFrom (object source);
  }
}