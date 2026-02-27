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

    public void Start ()
    {
      IStartable service = this;
      if ( !service.IsStarted ()) service.Start ();
    }

    public void Stop ()
    {
      (this as IService).Deactivate ();
      (this as IResettable).Reset ();
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