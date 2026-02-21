using Arunoki.Flow.Basics;

namespace Arunoki.Flow
{
  /// Hierarchical finite state machine.
  public class FlowStateMachine<TContext> : HubPart where TContext : class, IContext
  {
    public new TContext Context => base.Context as TContext;

    public FlowStateMachine (TContext context)
    {
      (this as IContextPart).Set (context);
    }
  }
}