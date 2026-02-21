using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public abstract class State<TEntity> : IState<TEntity> where TEntity : class
  {
    private TEntity entity;

    public TEntity Entity
    {
      get => entity;
      set
      {
        Guard.ThrowIfRewrite (entity, value);
        entity = value;
      }
    }

    public abstract void OnEnter ();
    public abstract void OnExit ();
    public abstract void OnUpdate ();
  }
}