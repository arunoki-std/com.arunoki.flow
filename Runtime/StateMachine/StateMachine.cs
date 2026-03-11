using Arunoki.Flow.Basics;

namespace Arunoki.Flow
{
  /// <summary> Hierarchical finite state machine. </summary>
  /// <para> Invoke <see cref="Update"/> method of the state machine manually from its <see cref="TContext"/> update loop. </para>
  public partial class StateMachine<TContext> : BaseServiceExplicit where TContext : class
  {
    protected internal readonly TContext Context;

    public StateMachine (TContext context)
    {
      Context = context;

      if (Context is not IDummy) AddStatesFrom (Context);
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