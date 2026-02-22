namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    public static IFsmBuilder<TEntity> GetBuilder (StateMachine<TEntity> stateMachine)
      => new Builder (stateMachine);

    internal readonly struct Builder : IFsmBuilder<TEntity>
    {
      private readonly StateMachine<TEntity> stateMachine;

      public Builder (StateMachine<TEntity> stateMachine)
      {
        this.stateMachine = stateMachine;
      }

      public void AddState<TState> () where TState : IState<TEntity>, new ()
      {
        stateMachine.CreateNode<TState> ();
      }

      public void AddRouter<TRouter> () where TRouter : IStateRouter<TEntity>, new ()
      {
        stateMachine.CreateRouter<TRouter> ();
      }

      public void GetStatesFrom (object source)
      {
        stateMachine.CreateStatesFrom (source);
      }

      public void GetRoutersFrom (object source)
      {
        stateMachine.CreateRoutersFrom (source);
      }

      public void GetAllFrom (object source)
      {
        stateMachine.CreateStatesFrom (source);
        stateMachine.CreateRoutersFrom (source);
      }
    }
  }
}