using Arunoki.Flow;

using System;

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

    public override void Where (Func<TElement, bool> condition, Action<TElement> action)
    {
      GetGroup ().Where (condition, action);
    }

    public override void Cast<T> (Func<T, bool> condition, Action<T> action)
    {
      GetGroup ().Cast (condition, action);
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