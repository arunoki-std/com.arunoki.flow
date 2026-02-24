using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    private readonly List<StateNode<TEntity>> pathA = new(16);
    private readonly List<StateNode<TEntity>> pathB = new(16);
    private StateNode<TEntity> root;
    private StateNode<TEntity> initialRoot;

    /// Invoke before starting state machine.
    protected void InitRoot<TState> () where TState : IState<TEntity>, new ()
    {
      if (!Nodes.TryGetValue (typeof(TState), out var node))
        node = CreateNode<TState> ();

      InitRoot (node);
    }

    /// Invoke before starting state machine.
    private void InitRoot (StateNode<TEntity> node)
    {
      if (node.Parent != null)
        throw new StateMachineException ($"State '{node.State.GetType ().Name}' with parent cant be root.");

      root = node;
      initialRoot = node;
    }

    protected override void OnStarted ()
    {
      base.OnStarted ();

      if (root == null)
        throw StateMachineException.RootIsNotDefined (this);

      root.EnterSelf ();
      root.EnterDefaultPath ();
    }

    public void Update ()
    {
      UpdateActivePath ();
    }

    private void UpdateActivePath ()
    {
      var node = root;
      while (true)
      {
        node.UpdateSelf ();

        if (node.ActiveChild == null) break;
        node = node.ActiveChild;
      }
    }

    public void Change<TState> () where TState : IState<TEntity>, new ()
    {
      StateNode<TEntity> node;
      try
      {
        node = Nodes [typeof(TState)];
      }
      catch (KeyNotFoundException)
      {
        throw StateMachineException.StateIsNotDefined (this, typeof(TState));
      }

      Change (node);
    }

    internal void Change (StateNode<TEntity> target)
    {
      if (target == null) throw new ArgumentNullException (nameof(target));

      // Root might be null on start.
      var previous = root?.GetActiveLeaf ();

      // Строим пути root->leaf и root->target
      StateNode<TEntity>.TryBuildPathToRoot (previous, pathA);
      StateNode<TEntity>.TryBuildPathToRoot (target, pathB);

      // Находим LCA (общий префикс)
      int i = 0;
      int max = Math.Min (pathA.Count, pathB.Count);
      while (i < max && ReferenceEquals (pathA [i], pathB [i])) i++;
      int lcaIndex = i - 1; // индекс LCA в путях

      // 1) Exit: от leaf до узла ПОСЛЕ LCA
      for (int a = pathA.Count - 1; a > lcaIndex; a--)
      {
        var node = pathA [a];
        node.ExitSelf ();

        // перед выходом leaf-узла хорошо бы "снять" активного ребенка у родителя
        var parent = node.Parent;
        if (parent != null && ReferenceEquals (parent.ActiveChild, node))
          parent.ClearActiveChild ();
      }

      // 2) Enter: от узла ПОСЛЕ LCA до target
      for (int b = lcaIndex + 1; b < pathB.Count; b++)
      {
        var node = pathB [b];
        var parent = node.Parent!;
        // активируем ребенка на родителе
        parent.SetActiveChild (node);
        node.EnterSelf ();
      }

      root = target.GetRoot ();

      // 3) Проваливаемся по default дочерним состояниям у target
      target.EnterDefaultPath ();
    }

    public bool Contains<TState> ()
    {
      var type = typeof(TState);

      foreach (var key in Nodes.Keys)
        if (ReferenceEquals (key, type) || type.IsAssignableFrom (key))
          return true;

      return false;
    }

    public bool IsActive<TState> ()
    {
      return root != null && root.IsAnyActive<TState> ();
    }
  }
}