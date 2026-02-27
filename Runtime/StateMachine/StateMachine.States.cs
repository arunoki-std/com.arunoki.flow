using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TEntity>
  {
    private readonly List<StateNode<TEntity>> pathA = new(16);
    private readonly List<StateNode<TEntity>> pathB = new(16);

    private StateNode<TEntity> pendingNode;

    protected override void OnActivate ()
    {
      base.OnActivate ();

      TrySetupNodes ();

      if (pendingNode != null)
      {
        TryBuildPathToRoot (pendingNode, pathA);
        currentRoot = pendingNode.GetRoot ();
        for (var i = 0; i < pathA.Count; i++)
        {
          var node = pathA [i];
          node.Parent?.SetActiveChild (node);
          node.EnterSelf ();
        }

        pendingNode.EnterDefaultPath ();
        pendingNode = null;
      }
      else
      {
        currentRoot = GetDefaultRoot ();
        currentRoot.EnterSelf ();
        currentRoot.EnterDefaultPath ();
      }
    }

    public void Update ()
    {
      if (!ApplyRequestIfAny ())
        UpdateActivePath ();
    }

    private void UpdateActivePath ()
    {
      var node = currentRoot;
      while (true)
      {
        node.UpdateSelf ();

        if (node.ActiveChild == null) break;
        node = node.ActiveChild;
      }
    }

    /// Will change state before next update.
    public void GoToRequest<TStateOrInterface> ()
    {
      if (IsActive<TStateOrInterface> ()) return;

      if (!TryGetNode<TStateOrInterface> (out this.pendingNode))
        throw StateMachineException.StateIsNotDefined (this, typeof(TStateOrInterface),
          "State change request won't work.");
    }

    /// Change state immediately. 
    public void GoTo<TStateOrInterface> ()
    {
      //TODO: MAKE GOTO AS PENDING REQUEST FOR ALL STATES

      if (TryGetNode<TStateOrInterface> (out var node))
        GoTo (node);

      else throw StateMachineException.StateIsNotDefined (this, typeof(TStateOrInterface));
    }

    internal void GoTo (StateNode<TEntity> target)
    {
      if (target == null) throw new ArgumentNullException (nameof(target));

      // Root might be null on start.
      var previous = currentRoot?.GetActiveLeaf ();

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

      currentRoot = target.GetRoot ();

      // 3) Проваливаемся по default дочерним состояниям у target
      target.EnterDefaultPath ();
    }

    protected bool ApplyRequestIfAny ()
    {
      if (pendingNode != null)
      {
        var node = pendingNode;
        pendingNode = null;

        GoTo (node);
        return true;
      }

      return false;
    }

    protected void TryExitActivePath ()
    {
      TryBuildPathToRoot (currentRoot?.GetActiveLeaf (), pathA);

      for (var i = pathA.Count - 1; i >= 0; i--)
        pathA [i].ExitSelf ();
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

      foreach (var key in NodesCache.Keys)
        if (ReferenceEquals (key, type) || type.IsAssignableFrom (key))
          return true;

      return false;
    }

    public bool IsActive<TStateOrInterface> ()
      => currentRoot != null && currentRoot.IsAnyActive<TStateOrInterface> ();

    public bool IsPending<TStateOrInterface> ()
      => pendingNode?.State is TStateOrInterface;
  }
}