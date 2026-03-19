using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class StateMachine<TContext>
  {
    private readonly List<StateNode<TContext>> pathA = new(16);
    private readonly List<StateNode<TContext>> pathB = new(16);

    private Type pendingState;

    protected override void OnActivate ()
    {
      base.OnActivate ();

      TrySetupNodes ();

      if (!ApplyRequestOnStart ())
      {
        EnterDefaultPathOnStart ();
      }
    }

    protected override void OnDeactivate ()
    {
      base.OnDeactivate ();

      TryExitActivePath ();
      currentRoot = null;
      pendingState = null;
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

    public void TryGoTo<TStateOrInterface> (bool pendingRequest = true)
    {
      if (Contains<TStateOrInterface> ())
        GoTo<TStateOrInterface> (pendingRequest);
    }

    ///  Change state on update if <see cref="pendingRequest"/> is true. 
    public void GoTo<TStateOrInterface> (bool pendingRequest = true)
    {
      pendingState = null;

      if (IsActiveLeaf<TStateOrInterface> ())
        return;

      pendingState = typeof(TStateOrInterface);

      if (!pendingRequest) ApplyRequestIfAny ();
    }

    internal void Change (StateNode<TContext> target)
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
      if (pendingState != null && (currentRoot == null || !currentRoot.IsTransitionLocked ()))
      {
        if (!TryGetNode (pendingState, out var node))
          throw StateMachineException.StateIsNotDefined ($"StateMachine<{typeof(TContext).Name}>", pendingState);

        pendingState = null;
        Change (node);

        return true;
      }

      return false;
    }

    protected bool ApplyRequestOnStart ()
    {
      if (pendingState == null) return false;

      if (!TryGetNode (pendingState, out var state))
        throw StateMachineException.StateIsNotDefined ($"StateMachine<{typeof(TContext).Name}>", pendingState);

      pendingState = null;

      TryBuildPathToRoot (state, pathA);
      currentRoot = state.GetRoot ();

      for (var i = 0; i < pathA.Count; i++)
      {
        var node = pathA [i];
        node.Parent?.SetActiveChild (node);
        node.EnterSelf ();
      }

      state.EnterDefaultPath ();
      return true;
    }

    protected void EnterDefaultPathOnStart ()
    {
      currentRoot = GetDefaultRoot ();
      currentRoot.EnterSelf ();
      currentRoot.EnterDefaultPath ();
    }

    protected void TryExitActivePath ()
    {
      TryBuildPathToRoot (currentRoot?.GetActiveLeaf (), pathA);

      for (var i = pathA.Count - 1; i >= 0; i--)
        pathA [i].ExitSelf ();
    }

    /// Путь от root до этого узла.
    private static void TryBuildPathToRoot (StateNode<TContext> node, List<StateNode<TContext>> buffer)
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

    public IState<TContext> GetActiveLeaf ()
      => currentRoot?.GetActiveLeaf ().State;

    public bool IsActive<TStateOrInterface> ()
      => currentRoot != null && currentRoot.IsAnyActive<TStateOrInterface> ();

    public bool IsActiveLeaf<TStateOrInterface> ()
      => currentRoot?.GetActiveLeaf ().State is TStateOrInterface;

    public bool HasChangeRequest () => pendingState != null;
  }
}