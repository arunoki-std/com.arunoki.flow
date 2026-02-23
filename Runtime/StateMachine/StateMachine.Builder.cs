namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    protected readonly struct Builder : IStateBuilder<TEntity>
    {
      private readonly StateMachine<TEntity> stateMachine;

      public Builder (StateMachine<TEntity> stateMachine)
      {
        this.stateMachine = stateMachine;
      }

      public void InitRoot<TState> () where TState : IState<TEntity>, new ()
      {
        stateMachine.InitRoot<TState> ();
      }

      public void AddState<TState> () where TState : IState<TEntity>, new ()
      {
        stateMachine.CreateNode<TState> ();
      }

      public void InitRouter<TRouter> () where TRouter : IStateRouter<TEntity>, new ()
      {
        stateMachine.InitRouter<TRouter> ();
      }

      public void ProduceStatesFrom (object source)
      {
        if (source == stateMachine.Entity)
          throw new StateMachineException (
            $"States were already produced from '{nameof(TEntity)}', no need to do it manually.");

        stateMachine.CreateStatesFrom (source);
      }

      public void ProduceRoutersFrom (object source)
      {
        if (source == stateMachine.Entity)
          throw new StateMachineException (
            $"Routers were already produced from '{nameof(TEntity)}', no need to do it manually.");

        stateMachine.CreateRoutersFrom (source);
      }

      public void ProduceAllFrom (object source)
      {
        ProduceStatesFrom (source);
        ProduceRoutersFrom (source);
      }
    }
  }
}