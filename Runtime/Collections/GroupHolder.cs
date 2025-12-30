using Arunoki.Flow.Utils;

using System;
using System.Collections.Generic;

namespace Arunoki.Collections
{
  public class GroupHolder<TElement> : BaseGroup<TElement>
  {
    private readonly List<BaseGroup<TElement>> groups = new();

    protected virtual void TrySetTargetHandler (object groupHandler)
    {
      if (groupHandler is IGroupHandler<TElement> e)
        SetTargetHandler (e);
    }

    protected virtual void SetTargetHandler (IGroupHandler<TElement> groupHandler)
    {
      (this as IGroupHandler<TElement>).TargetGroupHandler = groupHandler;
    }

    public virtual void AddGroupsFrom (object source)
    {
      foreach (var group in source.GetAllProperties<BaseGroup<TElement>> ())
        AddGroup (group);
    }

    public virtual void AddGroup (BaseGroup<TElement> group)
    {
      if (!groups.Contains (group))
      {
        groups.Insert (0, group);
      }
    }

    public virtual void ForEachGroup<TGroup> (Action<TGroup> action)
    {
      for (var i = groups.Count - 1; i >= 0; i--)
        if (groups [i] is TGroup group)
          action (group);
    }

    public virtual IEnumerable<TGroup> ForEachGroup<TGroup> ()
    {
      for (var i = groups.Count - 1; i >= 0; i--)
        if (groups [i] is TGroup group)
          yield return group;
    }

    public virtual void SelectGroup<TGroup> (Predicate<TGroup> condition, Action<TGroup> action)
    {
      for (var i = groups.Count - 1; i >= 0; i--)
        if (groups [i] is TGroup group && condition (group))
          action (group);
    }

    public override IEnumerable<T> GetAll<T> ()
    {
      for (var i = groups.Count - 1; i >= 0; i--)
        foreach (T element in groups [i].GetAll<T> ())
          yield return element;
    }

    public override IEnumerable<TElement> GetAll ()
    {
      for (var i = groups.Count - 1; i >= 0; i--)
        foreach (TElement element in groups [i].GetAll<TElement> ())
          yield return element;
    }

    public override IEnumerable<T> Where<T> (Predicate<T> condition)
    {
      for (var i = groups.Count - 1; i >= 0; i--)
        foreach (T element in groups [i].Where (condition))
          yield return element;
    }

    public override IEnumerable<TElement> Where (Predicate<TElement> condition)
    {
      for (var i = groups.Count - 1; i >= 0; i--)
        foreach (TElement element in groups [i].Where (condition))
          yield return element;
    }

    public override void Select (Predicate<TElement> condition, Action<TElement> action)
    {
      for (var i = groups.Count - 1; i >= 0; i--)
        groups [i].Select (condition, action);
    }

    public override void Select<T> (Predicate<T> condition, Action<T> action)
    {
      for (var i = groups.Count - 1; i >= 0; i--)
        groups [i].Select (condition, action);
    }

    public override void ForEach (Action<TElement> action)
    {
      for (var i = groups.Count - 1; i >= 0; i--)
        groups [i].ForEach (action);
    }

    public override void Dispose ()
    {
      groups.ForEach (collection => collection.Dispose ());
    }
  }
}