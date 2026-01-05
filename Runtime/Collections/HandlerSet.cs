using Arunoki.Collections;

using System;

namespace Arunoki.Flow.Misc
{
  public abstract class HandlerSet : Container<IHandler>, ISet<IHandler>, IHubPart, IBuilder
  {
    protected HandlerSet (IContainer<IHandler> targetContainer = null) : base (targetContainer)
    {
    }

    public FlowHub Hub { get; private set; }
    public IContext Context { get; private set; }
    protected abstract ISet<IHandler> GetConcreteSet ();

    protected virtual void Init (FlowHub hub)
    {
      Hub = hub;
    }

    IContext IContextPart.Get () => Context;
    void IContextPart.Set (IContext context) => Context = context;

    FlowHub IHubPart.Hub => Hub;
    void IHubPart.Init (FlowHub hub) => Init (hub);

    public abstract void Produce (object element);
    public abstract bool IsConsumable (object element);

    public void RemoveWhere (Func<IHandler, bool> condition)
      => GetConcreteSet ().RemoveWhere (condition);

    public void ForEach (Action<IHandler> action)
      => GetConcreteSet ().ForEach (action);

    public void Cast<T> (Action<T> action)
      => GetConcreteSet ().Cast (action);

    public void Cast<T> (Func<T, bool> condition, Action<T> action)
      => GetConcreteSet ().Cast (condition, action);

    public void Where (Func<IHandler, bool> condition, Action<IHandler> action)
      => GetConcreteSet ().Where (condition, action);

    public virtual void Clear ()
    {
      GetConcreteSet ().Clear ();
    }
  }
}