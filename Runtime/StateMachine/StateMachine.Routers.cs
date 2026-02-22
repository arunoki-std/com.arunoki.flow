using Arunoki.Collections.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    protected readonly List<IStateRouter<TEntity>> Routers = new(16);

    protected void CreateRoutersFrom (object source)
    {
      if (source is IDummy) return;

      foreach (Type routerType in source.GetType ().GetNestedTypes<IStateRouter<TEntity>> ())
        Bind ((IStateRouter<TEntity>) Activator.CreateInstance (routerType));
    }

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
        throw new BuildOperationException (
          $"{nameof(Bind)} must be called to bind the router '{router}' before the state machine is initialized. " +
          $"Try callback '{nameof(onPreInit)}' for this case.");
    }
  }
}