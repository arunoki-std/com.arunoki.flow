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
        AddRouter ((IStateRouter<TEntity>) Activator.CreateInstance (routerType));
    }

    protected virtual void InitRouters ()
    {
      for (var index = 0; index < Routers.Count; index++)
        Hub.Build (Routers [index]);
    }

    protected virtual void ClearRouters ()
    {
      for (var index = 0; index < Routers.Count; index++)
        Hub.Clear (Routers [index]);
    }

    protected void CreateRouter<TRouter> () where TRouter : IStateRouter<TEntity>, new ()
    {
      AddRouter (new TRouter ());
    }

    protected void AddRouter (IStateRouter<TEntity> router)
    {
      Routers.Add (router);
      router.Machine = this;
      router.Entity = Entity;
    }
  }
}