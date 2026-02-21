namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    protected virtual void OnInitRouters ()
    {
      for (var index = 0; index < Routers.Count; index++)
        OnInitRouter (Routers [index]);
    }

    protected virtual void OnInitRouter (IStateRouter<TEntity> router)
    {
      Hub.Build (router);
    }

    public void Bind<TRouter> () where TRouter : IStateRouter<TEntity>, new ()
    {
      Bind (new TRouter ());
    }

    public void Bind (IStateRouter<TEntity> router)
    {
      if (Routers.Contains (router)) return;

      Routers.Add (router);
      router.Machine = this;
      router.Entity = Entity;

      if (IsInitialized ())
        throw new BuildOperationException ($"{nameof(Bind)} '{nameof(router)}' before state machine was initialized.");
    }
  }
}