using System;
using System.Collections.Generic;

namespace Arunoki.Collections
{
  public class KeyTypeGroups<TElement> : BaseGroup<TElement>
  {
    protected internal Dictionary<Type, Group<TElement>> Cache = new();

    public KeyTypeGroups ()
    {
    }

    public KeyTypeGroups (IGroupHandler<TElement> groupHandler) : base (groupHandler)
    {
    }

    protected virtual void Add (Type keyType, params TElement [] elements)
    {
      if (!Cache.TryGetValue (keyType, out Group<TElement> group))
      {
        group = new Group<TElement> ();
        Cache.Add (keyType, group);
      }

      foreach (var element in elements)
      {
        group.Add (element);
        OnAdded (element);
      }
    }

    protected internal virtual void Remove (Type keyType)
    {
      if (Cache.TryGetValue (keyType, out Group<TElement> group))
      {
        group.ForEach (OnRemoved);
        group.Dispose ();
      }
    }

    protected internal virtual void Remove (TElement element)
    {
      foreach (var group in Cache.Values)
      {
        var i = group.Elements.IndexOf (element);
        if (i > -1)
        {
          group.Elements.RemoveAt (i);
          OnRemoved (element);
          return;
        }
      }
    }

    public override IEnumerable<T> GetAll<T> ()
    {
      foreach (var group in Cache.Values)
        for (var i = group.Elements.Count - 1; i >= 0; i--)
          if (group.Elements [i] is T match)
            yield return match;
    }

    public override IEnumerable<TElement> GetAll ()
    {
      foreach (var group in Cache.Values)
        for (var i = group.Elements.Count - 1; i >= 0; i--)
          yield return group.Elements [i];
    }

    public override IEnumerable<T> Where<T> (Predicate<T> condition)
    {
      foreach (var group in Cache.Values)
        for (var i = group.Elements.Count - 1; i >= 0; i--)
          if (group.Elements [i] is T match && condition (match))
            yield return match;
    }

    public override IEnumerable<TElement> Where (Predicate<TElement> condition)
    {
      foreach (var group in Cache.Values)
        for (var i = group.Elements.Count - 1; i >= 0; i--)
          if (condition (group.Elements [i]))
            yield return group.Elements [i];
    }

    public override void Select (Predicate<TElement> condition, Action<TElement> action)
    {
      foreach (var group in Cache.Values)
        for (var i = group.Elements.Count - 1; i >= 0; i--)
          if (condition (group.Elements [i]))
            action (group.Elements [i]);
    }

    public override void Select<T> (Predicate<T> condition, Action<T> action)
    {
      foreach (var group in Cache.Values)
        for (var i = group.Elements.Count - 1; i >= 0; i--)
          if (group.Elements [i] is T match && condition (match))
            action (match);
    }

    public override void ForEach (Action<TElement> action)
    {
      foreach (var group in Cache.Values)
        for (var i = group.Elements.Count - 1; i >= 0; i--)
          action (group.Elements [i]);
    }

    public Group<TElement> Get<TKeyType> ()
    {
      return Cache [typeof(TKeyType)];
    }

    public override void Dispose ()
    {
      foreach (var keyType in Cache.Keys)
        Remove (keyType);
    }
  }
}