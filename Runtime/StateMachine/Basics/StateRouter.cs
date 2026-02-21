using Arunoki.Flow.Utilities;

namespace Arunoki.Flow
{
  public abstract class StateRouter<TEntity> : IStateRouter<TEntity> where TEntity : class
  {
    private TEntity entity;
    private StateMachine<TEntity> machine;

    public TEntity Entity
    {
      get => entity;
      set
      {
        Guard.ThrowIfRewrite (entity, value);
        entity = value;
      }
    }

    public StateMachine<TEntity> Machine
    {
      get => machine;
      set
      {
        Guard.ThrowIfRewrite (machine, value);
        machine = value;
      }
    }
  }
}