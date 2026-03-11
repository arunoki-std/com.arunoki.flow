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

    public StateNode<TEntity> GetActiveLeaf ()
    {
      var node = this;

      while (node.ActiveChild != null)
        node = node.ActiveChild;

      return node;
    }

    public StateNode<TEntity> GetRoot ()
      => Parent != null ? Parent.GetRoot () : this;

    /// <summary>  From state to active leaf.  </summary>
    public bool IsAnyActive<TStateOrInterface> ()
    {
      if (State is TStateOrInterface) return true;
      return ActiveChild != null && ActiveChild.IsAnyActive<TStateOrInterface> ();
    }

    /// <summary>  From state to parent. </summary>
    public bool IsAnyParent<TStateOrInterface> ()
    {
      if (State is TStateOrInterface) return true;
      return Parent != null && Parent.IsAnyParent<TStateOrInterface> ();
    }

    /// <summary>  From state to any from default path. </summary>
    public bool IsAnyDefault<TStateOrInterface> ()
    {
      if (State is TStateOrInterface) return true;
      return DefaultChild != null && DefaultChild.IsAnyParent<TStateOrInterface> ();
    }

    public bool IsRoot () => Parent == null;

    public bool IsTransitionLocked ()
    {
      if (State.IsLocked ()) return true;
      return ActiveChild?.IsTransitionLocked () ?? false;
    }

    public bool IsTypeParent (Type other)
    {
      var state = State.GetType ();
      return ReferenceEquals (state, other) || other.IsSubclassOf (state) || other.IsAssignableFrom (state);
    }
  }
}