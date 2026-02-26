using Arunoki.Flow.Basics;

namespace Arunoki.Flow
{
  /// Hierarchical finite state machine.
  public partial class StateMachine<TEntity> : BaseService where TEntity : class
  {
    protected internal readonly TEntity Entity;

    public StateMachine (TEntity entity)
    {
      Entity = entity;

      if (Entity is not IDummy) AddStatesFrom (Entity);
    }

    protected override void OnReset ()
    {
      root = initialRoot;
      base.OnReset ();
    }

    public virtual void Dispose ()
    {
      Nodes.Clear ();
    }
  }
}