using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public abstract class State<TEntity> : IState<TEntity> where TEntity : class
  {
    private readonly bool isDefault;
    private readonly bool isRoot;
    private TEntity entity;

    protected State (bool isDefault, bool isRoot)
    {
      this.isDefault = isDefault;
      this.isRoot = isRoot;
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
    bool IState<TEntity>.IsRoot () => isRoot;
  }
}