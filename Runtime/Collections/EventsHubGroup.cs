using Arunoki.Flow;

using System;

namespace Arunoki.Collections
{
  public abstract class EventsHubGroup<TElement> : ElementHandler<TElement>, IEventsHubPart
  {
    protected EventsHubGroup (IElementHandler<TElement> targetHandler = null) : base (targetHandler)
    {
    }

    public IEventsContext Context => Hub.Context;

    public EventHub Hub { get; private set; }

    void IEventsHubPart.Init (EventHub hub) => Init (hub);

    protected virtual void Init (EventHub hub)
    {
      Hub = hub;
    }

    protected abstract ISet<TElement> GetSet ();

    public void Where (Func<TElement, bool> condition, Action<TElement> action)
    {
      GetSet ().Where (condition, action);
    }

    public void Cast<T> (Func<T, bool> condition, Action<T> action)
    {
      GetSet ().Cast (condition, action);
    }

    public void ForEach (Action<TElement> action)
    {
      GetSet ().ForEach (action);
    }

    public void Clear ()
    {
      GetSet ().Clear ();
    }
  }
}