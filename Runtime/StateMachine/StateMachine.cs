using Arunoki.Flow.Basics;

namespace Arunoki.Flow
{
  /// Hierarchical finite state machine.
  public partial class StateMachine<TEntity> : BaseServiceExplicit where TEntity : class
  {
    protected internal readonly TEntity Entity;

    public StateMachine (TEntity entity)
    {
      Entity = entity;

      if (Entity is not IDummy) AddStatesFrom (Entity);
    }

    public void Activate () => (this as IService).Activate ();
    public void Deactivate () => (this as IService).Deactivate ();

    public virtual void Dispose ()
    {
      Deactivate ();
      NodesCache.Clear ();
      Nodes.Clear ();
    }
  }
}