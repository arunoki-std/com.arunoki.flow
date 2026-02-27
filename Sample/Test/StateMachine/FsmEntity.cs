namespace Arunoki.Flow.Sample
{
  public partial class FsmEntity : IContext, IPipeline
  {
    public Signal<TimeoutEvent> Timeout { get; } = new();

    private StateMachine<FsmEntity> States { get; }

    public FsmEntity ()
    {
      States = new StateMachine<FsmEntity> (this);
    }

    public void Update ()
    {
      States.Update ();
    }

    public bool IsState<TState> () where TState : IState<FsmEntity>
      => States.IsActive<TState> ();
  }
}