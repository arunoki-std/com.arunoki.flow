using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    private readonly List<StateNode<TEntity>> pathA = new(16);
    private readonly List<StateNode<TEntity>> pathB = new(16);
    private StateNode<TEntity> root;

    protected override void OnStarted ()
    {
      base.OnStarted ();

      if (root == null)
        throw new InvalidOperationException ($"Root state is not defined at '{this}'.");

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

    public void ChangeState<TState> () where TState : IState<TEntity>, new ()
    {
      var nextNode = Nodes [typeof(TState)];
      if (nextNode == root.GetActiveLeaf ()) return;

      ChangeState (nextNode);
    }

    internal void ChangeState (StateNode<TEntity> target)
    {
      if (target == null) throw new ArgumentNullException (nameof(target));

      var current = root.GetActiveLeaf ();

      // Строим пути root->leaf и root->target
      StateNode<TEntity>.BuildPathToRoot (current, pathA);
      StateNode<TEntity>.BuildPathToRoot (target, pathB);

      // Находим LCA (общий префикс)
      int i = 0;
      int max = Math.Min (pathA.Count, pathB.Count);
      while (i < max && ReferenceEquals (pathA [i], pathB [i])) i++;
      int lcaIndex = i - 1; // индекс LCA в путях

      // 1) Exit: от leaf до узла ПОСЛЕ LCA
      for (int a = pathA.Count - 1; a > lcaIndex; a--)
      {
        var node = pathA [a];
        // перед выходом leaf-узла хорошо бы "снять" активного ребенка у родителя
        node.ExitSelf ();

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

      // 3) Проваливаемся по default дочерним состояниям у target
      target.EnterDefaultPath ();
    }

    protected void SetRoot<TState> () where TState : IState<TEntity>, new ()
    {
      if (!Nodes.TryGetValue (typeof(TState), out var node))
        node = CreateNode<TState> ();

      SetRoot (node);
    }

    private void SetRoot (StateNode<TEntity> node)
    {
      Guard.ThrowIfRewrite (root, node);
      root = node;
    }
  }
}