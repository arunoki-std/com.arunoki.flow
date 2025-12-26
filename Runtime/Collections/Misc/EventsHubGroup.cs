using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Collections
{
  public abstract class EventsHubGroup<TElement> : BaseGroup<TElement>, IEventsHubPart
  {
    public IEventsContext Context => Hub.Context;

    public EventHub Hub { get; private set; }

    void IEventsHubPart.Init (EventHub hub) => Init (hub);

    protected virtual void Init (EventHub hub)
    {
      Hub = hub;
    }

    protected abstract BaseGroup<TElement> GetGroup ();

    public override IEnumerable<T> GetAll<T> ()
    {
      foreach (var element in GetGroup ().GetAll<T> ())
        yield return element;
    }

    public override IEnumerable<TElement> GetAll ()
    {
      foreach (var element in GetGroup ().GetAll ())
        yield return element;
    }

    public override IEnumerable<T> Where<T> (Predicate<T> condition)
    {
      foreach (var element in GetGroup ().Where (condition))
        yield return element;
    }

    public override IEnumerable<TElement> Where (Predicate<TElement> condition)
    {
      foreach (var element in GetGroup ().Where (condition))
        yield return element;
    }

    public override void Select (Predicate<TElement> condition, Action<TElement> action)
    {
      GetGroup ().Select (condition, action);
    }

    public override void Select<T> (Predicate<T> condition, Action<T> action)
    {
      GetGroup ().Select (condition, action);
    }

    public override void ForEach (Action<TElement> action)
    {
      GetGroup ().ForEach (action);
    }

    public override void Dispose ()
    {
      GetGroup ().Dispose ();
    }
  }
}