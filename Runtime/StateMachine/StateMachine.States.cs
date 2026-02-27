using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    private readonly List<StateNode<TEntity>> pathA = new(16);
    private readonly List<StateNode<TEntity>> pathB = new(16);

    protected override void OnActivate ()
    {
      base.OnActivate ();

      SetupNodes ();

      if (root == null && !TryFindDefaultRoot (out root))
      {
        throw StateMachineException.RootIsNotDefined (this);
      }

      root.EnterSelf ();
      root.EnterDefaultPath ();
    }

    protected override void OnDeactivate ()
    {
      base.OnDeactivate ();

      TryBuildPathToRoot (root.GetActiveLeaf (), pathA);

      for (var i = pathA.Count - 1; i >= 0; i--)
        pathA [i].ExitSelf ();
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

    public bool TryGoTo<TStateOrInterface> ()
    {
      if (TryGetNode<TStateOrInterface> (out var node))
      {
        GoTo (node);
        return true;
      }

      return false;
    }

    public void GoTo<TStateOrInterface> ()
    {
      if (TryGetNode<TStateOrInterface> (out var node))
        GoTo (node);

      else throw StateMachineException.StateIsNotDefined (this, typeof(TStateOrInterface));
    }

    internal void GoTo (StateNode<TEntity> target)
    {
      if (target == null) throw new ArgumentNullException (nameof(target));

      // Root might be null on start.
      var previous = root?.GetActiveLeaf ();

      // Строим пути root->leaf и root->target
      TryBuildPathToRoot (previous, pathA);
      TryBuildPathToRoot (target, pathB);

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
        // активируем ребенка на родителе
        node.Parent?.SetActiveChild (node);
        node.EnterSelf ();
      }

      root = target.GetRoot ();

      // 3) Проваливаемся по default дочерним состояниям у target
      target.EnterDefaultPath ();
    }

    /// Путь от root до этого узла.
    private static void TryBuildPathToRoot (StateNode<TEntity> node, List<StateNode<TEntity>> buffer)
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

    public bool Contains<TStateOrInterface> ()
    {
      var type = typeof(TStateOrInterface);

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