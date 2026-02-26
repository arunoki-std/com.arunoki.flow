namespace Arunoki.Flow.Sample
{
  public partial class FsmEntity : IContext, IPipeline
  {
    public Signal<TimeoutEvent> Timeout { get; } = new();

    private StateMachine<FsmEntity> StateMachine { get; }

    public FsmEntity ()
    {
      StateMachine = new StateMachine<FsmEntity> (this);
    }

    public void Update ()
    {
      StateMachine.Update ();
    }

    public bool IsState<TState> () where TState : IState<FsmEntity>
      => StateMachine.IsActive<TState> ();
  }
}