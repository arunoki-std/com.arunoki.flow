using Arunoki.Flow.Basics;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  /// Hierarchical finite state machine.
  public partial class StateMachine<TEntity> : BaseServiceExplicit where TEntity : class
  {
    private readonly Action<StateMachine<TEntity>> onPreInit;
    protected readonly List<IStateRouter<TEntity>> Routers = new(16);
    protected internal readonly TEntity Entity;
    protected readonly FlowHub Hub;

    public StateMachine (TEntity entity, FlowHub hub, Action<StateMachine<TEntity>> onPreInit = null)
    {
      TargetService = new ServiceContainer<IStateRouter<TEntity>> (Routers);
      Entity = entity;
      Hub = hub;

      this.onPreInit = onPreInit;
    }

    protected override void OnInitialized ()
    {
      onPreInit?.Invoke (this);

      OnInitRouters ();

      base.OnInitialized ();
    }
  }
}