using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public abstract class State<TEntity> : IState<TEntity> where TEntity : class
  {
    private readonly bool isDefault;
    private TEntity entity;

    protected State (bool isDefault)
    {
      this.isDefault = isDefault;
    }

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

    bool IState<TEntity>.IsDefault () => isDefault;
  }
}