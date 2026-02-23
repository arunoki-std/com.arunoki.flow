namespace Arunoki.Flow
{
  /// <summary> This is part of the state machine initialization step. </summary>
  /// <summary> Implement by <see cref="IStateRouter{TEntity}"/> or by <see cref="TEntity"/> itself. </summary>
  /// <summary> Add states, create routers, define root state. </summary> 
  public interface IStateInitializer<TEntity> where TEntity : class
  {
    void OnInit (IStateBuilder<TEntity> builder);
  }
}