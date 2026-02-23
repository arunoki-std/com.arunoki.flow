using Arunoki.Collections.Utilities;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    internal Dictionary<Type, StateNode<TEntity>> Nodes = new(16);

    protected virtual void InitStates ()
    {
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
          var parentType = parentNode.State.GetType ();
          if (ReferenceEquals (parentType, requiredType) || requiredType.IsAssignableFrom (parentType))
          {
            parentNode.AddChild (childNode, childNode.State.IsDefault ());
            targetFound = true;
            break;
          }
        }

        if (!targetFound)
          throw new StateMachineException ($"Parent for state '{childNode.State}' not found.");
      }

      foreach (var node in Nodes.Values)
        if (node.State is IInitializable state && !state.IsInitialized ())
          state.Initialize ();
    }

    protected void CreateStatesFrom (object source)
    {
      if (source == null) throw new ArgumentNullException (nameof(source));
      if (source is IDummy) return;

      foreach (Type stateType in source.GetType ().GetNestedTypes<IState<TEntity>> ())
        CreateNode (stateType);
    }

    private StateNode<TEntity> CreateNode<TState> () where TState : IState<TEntity>, new ()
      => CreateNode (typeof(TState));

    private StateNode<TEntity> CreateNode (Type stateType)
    {
      if (Nodes.TryGetValue (stateType, out var node))
        return node;

      var state = CreateState (stateType);
      node = new StateNode<TEntity> (stateType.Name, state);
      Nodes.Add (stateType, node);

      return node;
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
        throw new InvalidOperationException ($"'{stateType}' doesn't implement '{nameof(IState<TEntity>)}'.");
      }

      if (Guard.IsNull (state.Entity)) state.Entity = Entity;
      return state;
    }
  }
}