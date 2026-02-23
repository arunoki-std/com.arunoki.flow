using Arunoki.Flow.Basics;

namespace Arunoki.Flow
{
  /// Hierarchical finite state machine.
  public partial class StateMachine<TEntity> : BaseServiceExplicit where TEntity : class
  {
    protected internal readonly TEntity Entity;
    protected readonly FlowHub Hub;

    public StateMachine (TEntity entity, FlowHub hub)
    {
      TargetService = new ServiceContainer<IStateRouter<TEntity>> (Routers);
      Entity = entity;
      Hub = hub;
    }

    protected override void OnInitialized ()
    {
      CreateStatesFrom (Entity);
      CreateRoutersFrom (Entity);

      if (Entity is IStateInitializer<TEntity> e)
        e.OnInit (new Builder (this));

      InitStates ();

      // init routers
      base.OnInitialized ();

      BuildRouters ();
    }

    protected override void OnReset ()
    {
      root = initialRoot;
      base.OnReset ();
    }

    public virtual void Dispose ()
    {
      ClearRouters ();
    }
  }
}