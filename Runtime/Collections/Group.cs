using System;
using System.Collections.Generic;

namespace Arunoki.Collections
{
  public class Group<TElement> : BaseGroup<TElement>
  {
    protected internal List<TElement> Elements = new();

    protected internal virtual void Add (TElement element)
    {
      Elements.Insert (0, element);
      OnAdded (element);
    }

    protected internal virtual void Remove (TElement element)
    {
      try
      {
        for (var i = Elements.Count - 1; i >= 0; i--)
        {
          if (element.Equals (Elements [i]))
          {
            Elements.RemoveAt (i);
            OnRemoved (element);
          }
        }
      }
      catch (NullReferenceException)
      {
        RemoveNulls ();
      }
    }

    protected void RemoveNulls ()
    {
      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is null)
          Elements.RemoveAt (i);
    }

    public override IEnumerable<T> GetAll<T> ()
    {
      if (this is T self)
        yield return self;

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is T match)
          yield return match;
    }

    public override IEnumerable<TElement> GetAll ()
    {
      for (var i = Elements.Count - 1; i >= 0; i--)
        yield return Elements [i];
    }

    public override IEnumerable<TElement> Where (Predicate<TElement> condition)
    {
      for (var i = Elements.Count - 1; i >= 0; i--)
        if (condition (Elements [i]))
          yield return Elements [i];
    }

    public override IEnumerable<T> Where<T> (Predicate<T> condition)
    {
      if (this is T self && condition (self))
        yield return self;

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is T match && condition (match))
          yield return match;
    }

    public override void Select (Predicate<TElement> condition, Action<TElement> action)
    {
      for (var i = Elements.Count - 1; i >= 0; i--)
        if (condition (Elements [i]))
          action (Elements [i]);
    }

    public override void Select<T> (Predicate<T> condition, Action<T> action)
    {
      if (this is T self && condition (self))
        action (self);

      for (var i = Elements.Count - 1; i >= 0; i--)
        if (Elements [i] is T match && condition (match))
          action (match);
    }

    public override void ForEach (Action<TElement> action)
    {
      for (var i = Elements.Count - 1; i > -1 && i < Elements.Count; i--)
        action (Elements [i]);
    }

    public override void Dispose ()
    {
      for (var i = Elements.Count - 1; i >= 0; i--)
      {
        Remove (Elements [i]);
      }

      Elements.Clear ();
      Elements = new();
    }
  }
}