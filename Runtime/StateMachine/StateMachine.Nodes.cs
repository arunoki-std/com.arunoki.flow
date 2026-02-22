using Arunoki.Collections.Utilities;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    internal Dictionary<Type, StateNode<TEntity>> Nodes = new(16);

    protected void CreateNodesFrom (object source)
    {
      if (source is IDummy) return;

      foreach (Type stateType in source.GetType ().GetNestedTypes<IState<TEntity>> ())
        CreateNode (stateType);
    }

    private StateNode<TEntity> CreateNode (Type stateType)
    {
      if (Nodes.TryGetValue (stateType, out var node))
        return node;

      var state = CreateState (stateType);
      node = new StateNode<TEntity> (stateType.Name, state);
      Nodes.Add (stateType, node);

      if (TryGetParentNode (stateType, out var parentNode))
        parentNode.AddChild (node, state.IsDefault ());

      return node;
    }

    private bool TryGetParentNode (Type childState, out StateNode<TEntity> parentNode)
    {
      parentNode = null;
      if (childState.TryGetConcreteParent<IState<TEntity>> (out Type parentState))
      {
        if (Nodes.TryGetValue (parentState, out parentNode))
          return true;

        parentNode = CreateNode (parentState);
      }

      return parentNode != null;
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