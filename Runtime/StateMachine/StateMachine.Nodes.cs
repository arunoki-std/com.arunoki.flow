using Arunoki.Collections.Utilities;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    internal Dictionary<Type, StateNode<TEntity>> NodesCache { get; } = new(16);
    internal List<StateNode<TEntity>> Nodes { get; } = new(16);

    private StateNode<TEntity> currentRoot;
    private bool nodesReady;

    protected virtual void TrySetupNodes ()
    {
      if (nodesReady) return;
      nodesReady = true;

      var parents = new List<StateNode<TEntity>> (NodesCache.Count);
      var children = new List<StateNode<TEntity>> (NodesCache.Count);

      foreach (var node in Nodes)
      {
        if (node.State.IsSubState ()) children.Add (node);
        else parents.Add (node);
      }

      foreach (var childNode in children)
      {
        var requiredParent = childNode.State.GetParentType ();
        bool targetFound = false;

        foreach (var parentNode in parents)
        {
          var pureParent = parentNode.State.GetType ();

          if (ReferenceEquals (pureParent, requiredParent) || requiredParent.IsSubclassOf (pureParent) ||
              requiredParent.IsAssignableFrom (pureParent))
          {
            parentNode.AddChild (childNode, childNode.State.IsDefault ());
            targetFound = true;
            break;
          }
        }

        if (!targetFound)
          throw new StateMachineException ($"Parent for state '{childNode}' not found.");
      }

      foreach (var node in Nodes)
        if (node.State is IInitializable state && !state.IsInitialized ())
          state.Initialize ();
    }

    public void AddState<TState> () where TState : IState<TEntity>, new ()
    {
      CreateNode (typeof(TState));
    }

    public void AddState (IState<TEntity> state)
    {
      CreateNode (state.GetType (), state);
    }

    public void AddStatesFrom (object stateSource)
    {
      if (stateSource == null) throw new ArgumentNullException (nameof(stateSource));
      if (stateSource is IDummy) return;

      foreach (Type stateType in stateSource.GetType ().GetNestedTypes<IState<TEntity>> ())
        CreateNode (stateType);
    }

    private void CreateNode (Type stateType)
    {
      if (NodesCache.ContainsKey (stateType)) return;

      CreateNode (stateType, CreateState (stateType));
    }

    private void CreateNode (Type stateType, IState<TEntity> state)
    {
      var node = new StateNode<TEntity> (stateType.Name, state);
      NodesCache.Add (stateType, node);
      Nodes.Add (node);
    }

    private IState<TEntity> CreateState (Type stateType)
    {
      IState<TEntity> state;
      try
      {
        state = (IState<TEntity>) Activator.CreateInstance (stateType);
      }
      catch (InvalidCastException)
      {
        throw new StateMachineException (
          $"Can't create state '{stateType}'. Class doesn't implement '{nameof(IState<TEntity>)}'.");
      }

      if (Guard.IsNull (state.Entity)) state.Entity = Entity;
      return state;
    }

    private bool TryGetNode<TStateOrInterface> (out StateNode<TEntity> node)
      => TryGetNode (typeof(TStateOrInterface), out node);

    private bool TryGetNode (Type stateType, out StateNode<TEntity> node)
    {
      if (!stateType.IsInterface)
      {
        return NodesCache.TryGetValue (stateType, out node);
      }

      foreach (var pair in NodesCache)
      {
        if (stateType.IsAssignableFrom (pair.Key))
        {
          node = pair.Value;
          return true;
        }
      }

      node = null;
      return false;
    }

    /// First default state without parent would be defined as root state.
    private StateNode<TEntity> GetDefaultRoot ()
    {
      foreach (var node in Nodes)
      {
        if (node.IsRoot () && node.State.IsDefault ())
          return node;
      }

      throw StateMachineException.RootIsNotDefined (this, "Default root state not found.");
    }

    protected bool IsAssignableOrEquals (Type stateType, Type typeOrInterface)
    {
      return ReferenceEquals (stateType, typeOrInterface) || typeOrInterface.IsAssignableFrom (stateType);
    }
  }
}