using Arunoki.Flow.Basics;

namespace Arunoki.Flow
{
  /// Hierarchical finite state machine.
  public partial class StateMachine<TEntity> : BaseServiceExplicit where TEntity : class
  {
    protected internal readonly TEntity Entity;

    public StateMachine (TEntity entity, bool autoInit = true)
    {
      Entity = entity;

      if (autoInit)
        (this as IInitializable).Initialize ();
    }

    protected override void OnInitialized ()
    {
      AddStatesFrom (Entity);


      InitStates ();

      // init routers
      base.OnInitialized ();
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