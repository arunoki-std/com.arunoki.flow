using Arunoki.Flow;

using System;
using System.Collections.Generic;

namespace Arunoki.Collections
{
  public abstract class EventsHubGroup<TElement> : BaseSet<TElement>, IEventsHubPart
  {
    public IEventsContext Context => Hub.Context;

    public EventHub Hub { get; private set; }

    void IEventsHubPart.Init (EventHub hub) => Init (hub);

    protected virtual void Init (EventHub hub)
    {
      Hub = hub;
    }

    protected abstract BaseSet<TElement> GetGroup ();

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

    public override void ForEach (Predicate<TElement> condition, Action<TElement> action)
    {
      GetGroup ().ForEach (condition, action);
    }

    public override void ForEachOf<T> (Predicate<T> condition, Action<T> action)
    {
      GetGroup ().ForEachOf (condition, action);
    }

    public override void ForEach (Action<TElement> action)
    {
      GetGroup ().ForEach (action);
    }

    public override void Clear ()
    {
      GetGroup ().Clear ();
    }
  }
}