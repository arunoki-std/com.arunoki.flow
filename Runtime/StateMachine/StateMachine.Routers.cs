using Arunoki.Collections.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    protected readonly List<IStateRouter<TEntity>> Routers = new(16);

    protected internal void CreateRoutersFrom (object source)
    {
      if (source == null) throw new ArgumentNullException (nameof(source));
      if (source is IDummy) return;

      foreach (Type routerType in source.GetType ().GetNestedTypes<IStateRouter<TEntity>> ())
        InitRouter ((IStateRouter<TEntity>) Activator.CreateInstance (routerType));
    }

    protected virtual void BuildRouters ()
    {
      for (var index = 0; index < Routers.Count; index++)
        Hub.Register (Routers [index]);
    }

    protected virtual void ClearRouters ()
    {
      for (var index = 0; index < Routers.Count; index++)
        Hub.Remove (Routers [index]);
    }

    protected void InitRouter<TRouter> () where TRouter : IStateRouter<TEntity>, new ()
    {
      InitRouter (new TRouter ());
    }

    protected void InitRouter (IStateRouter<TEntity> router)
    {
      if (IsInitialized ())
        throw StateMachineException.RouterRegistrationOrder (this, router);

      Routers.Add (router);
      router.Machine = this;
      router.Entity = Entity;

      if (router is IStateInitializer<TEntity> stateInitializer)
        stateInitializer.OnInit (new Builder (this));
    }
  }
}