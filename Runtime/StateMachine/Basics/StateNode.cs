using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  internal sealed class StateNode<TEntity> where TEntity : class
  {
    public readonly string Name;
    internal readonly IState<TEntity> State;

    public StateNode<TEntity> Parent { get; private set; }
    public StateNode<TEntity> DefaultChild { get; private set; }
    public StateNode<TEntity> ActiveChild { get; private set; }

    public List<StateNode<TEntity>> Children = new(8);

    public StateNode (string name, IState<TEntity> state)
    {
      Name = name;
      State = state;
    }

    public void EnterSelf ()
    {
      State.OnEnter ();
      ActiveChild = null;
    }

    public void ExitSelf ()
    {
      State.OnExit ();
      ActiveChild = null;
    }

    public void UpdateSelf ()
    {
      State.OnUpdate ();
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

    /// Путь от root до этого узла.
    public static void BuildPathToRoot (StateNode<TEntity> node, List<StateNode<TEntity>> buffer)
    {
      buffer.Clear ();
      while (node != null)
      {
        buffer.Add (node);
        node = node.Parent;
      }

      // buffer сейчас leaf->root, переворачиваем в root->leaf
      buffer.Reverse ();
    }

    public StateNode<TEntity> GetActiveLeaf ()
    {
      var node = this;

      while (node.ActiveChild != null)
        node = node.ActiveChild;

      return node;
    }

    public bool IsPathDefault ()
    {
      var cur = this;
      while (cur != null)
      {
        if (!cur.State.IsDefault ()) return false;
        cur = cur.Parent;
      }

      return true;
    }
  }
}