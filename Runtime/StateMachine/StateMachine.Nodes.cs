using Arunoki.Collections.Utilities;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    internal Dictionary<Type, StateNode<TEntity>> Nodes = new(16);

    private StateNode<TEntity> root;
    private StateNode<TEntity> initialRoot;
    private bool nodesReady;

    protected virtual void SetupNodes ()
    {
      if (nodesReady) return;
      nodesReady = true;

      var parents = new List<StateNode<TEntity>> (Nodes.Count);
      var children = new List<StateNode<TEntity>> (Nodes.Count);

      foreach (var node in Nodes.Values)
      {
        if (node.State.IsSubState ()) children.Add (node);
        else parents.Add (node);
      }

      foreach (var childNode in children)
      {
        var requiredType = childNode.State.GetParentType ();
        bool targetFound = false;

        foreach (var parentNode in parents)
        {
          if (IsAssignableOrEquals (parentNode.State.GetType (), requiredType))
          {
            parentNode.AddChild (childNode, childNode.State.IsDefault ());
            targetFound = true;
            break;
          }
        }

        if (!targetFound)
          throw new StateMachineException ($"Parent for state '{childNode}' not found.");
      }

      foreach (var node in Nodes.Values)
        if (node.State is IInitializable state && !state.IsInitialized ())
          state.Initialize ();
    }

    public void AddState<TState> () where TState : IState<TEntity>, new ()
    {
      CreateNode (typeof(TState));
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
      if (Nodes.ContainsKey (stateType)) return;

      Nodes.Add (stateType, new StateNode<TEntity> (stateType.Name, CreateState (stateType)));
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
    {
      var stateType = typeof(TStateOrInterface);
      if (!stateType.IsInterface)
      {
        return Nodes.TryGetValue (stateType, out node);
      }

      foreach (var pair in Nodes)
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

    private bool TryFindDefaultRoot (out StateNode<TEntity> root)
    {
      foreach (var node in Nodes.Values)
      {
        if (node.IsRoot () && node.State.IsDefault ())
        {
          root = node;
          return true;
        }
      }

      root = null;
      return false;
    }

    private bool TryGetRoot<TStateOrInterface> (out StateNode<TEntity> root)
    {
      var targetType = typeof(TStateOrInterface);

      foreach (var node in Nodes.Values)
      {
        if (!node.IsRoot ()) continue;
        if (IsAssignableOrEquals (node.State.GetType (), targetType))
        {
          root = node;
          return true;
        }
      }

      root = null;
      return false;
    }

    /// <summary>
    /// Invoke before starting state machine. 
    /// <para> Set root after adding states. </para>
    /// </summary>
    public void SetRoot<TStateOrInterface> ()
    {
      if (TryGetRoot<TStateOrInterface> (out var node))
        SetRoot (node);

      else throw StateMachineException.StateIsNotDefined (this, typeof(TStateOrInterface));
    }

    /// Invoke before starting state machine.
    private void SetRoot (StateNode<TEntity> node)
    {
      if (node.Parent != null)
        throw new StateMachineException (
          $"State '{node.State.GetType ().Name}' cant be root. Root must not have any parent");

      if (root?.ActiveChild != null)
        throw new StateMachineException (
          $"Can't set root state '{node.Name}' when it is already active '{root.Name}'.");

      root = node;
      initialRoot = node;
    }

    protected bool IsAssignableOrEquals (Type stateType, Type typeOrInterface)
    {
      return ReferenceEquals (stateType, typeOrInterface) || typeOrInterface.IsAssignableFrom (stateType);
    }
  }
}