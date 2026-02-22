#nullable enable

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  internal sealed class StateNode<TEntity> where TEntity : class
  {
    public readonly string Name;
    private readonly IState<TEntity> state;

    public StateNode<TEntity>? Parent { get; private set; }
    public StateNode<TEntity>? DefaultChild { get; private set; }
    public StateNode<TEntity>? ActiveChild { get; private set; }

    public List<StateNode<TEntity>> Children = new(8);

    public StateNode (string name, IState<TEntity> state)
    {
      Name = name;
      this.state = state;
    }

    public void EnterSelf ()
    {
      state.OnEnter ();
      ActiveChild = null;
    }

    public void ExitSelf ()
    {
      state.OnExit ();
      ActiveChild = null;
    }

    public void UpdateSelf ()
    {
      state.OnUpdate ();
    }

    public void AddChild (StateNode<TEntity> child, bool isDefault)
    {
      if (child == null) throw new ArgumentNullException (nameof(child));

      if (child.Parent != null)
        throw new InvalidOperationException ($"State '{child.Name}' is already a child of '{child.Parent.Name}'.");

      if (isDefault)
      {
        if (DefaultChild != null)
          throw new RewriteOperationException (
            $"Trying to rewrite default state '{DefaultChild.Name}' by '{child.Name}'.");

        DefaultChild = child;
      }

      Children.Add (child);
      child.Parent = this;
    }

    public void EnterDefaultPath ()
    {
      if (DefaultChild == null) return;

      SetActiveChild (DefaultChild);

      DefaultChild.EnterSelf ();
      DefaultChild.EnterDefaultPath ();
    }

    public void SetActiveChild (StateNode<TEntity> child)
    {
      if (child.Parent != this)
        throw new InvalidOperationException ($"State '{child.Name}' is not a child of '{Name}'.");

      ActiveChild = child;
    }

    public void ClearActiveChild ()
    {
      ActiveChild = null;
    }

    public StateNode<TEntity> GetActiveLeaf ()
    {
      var node = this;

      while (node.ActiveChild != null)
        node = node.ActiveChild;

      return node;
    }

    public bool IsRoot () => Parent == null;
  }
}