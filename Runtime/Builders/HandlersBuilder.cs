using Arunoki.Collections;
using Arunoki.Flow.Events.Core;

using System;

namespace Arunoki.Flow.Collections
{
  public class HandlersBuilder : BaseBuilderService<IHandler>, IContainer<Type>
  {
    protected internal readonly Set<IHandler> Set;
    protected internal readonly SetsTypeCollection<IHandler> KeySet;

    private IContainer<Type> rootKeyBuilder;

    private SubscriptionService subscriber;

    public HandlersBuilder () : this (null, null) { }

    public HandlersBuilder (IContainer<Type> rootKeyBuilder) : this (null, rootKeyBuilder) { }

    public HandlersBuilder (IContainer<IHandler> rootBuilder, IContainer<Type> rootKeyBuilder = null)
      : base (rootBuilder)
    {
      this.rootKeyBuilder = rootKeyBuilder;

      Set = new(this);
      KeySet = new(this, this);
    }

    public sealed override void Produce (IHandler handler)
    {
      base.Produce (handler);

      Set.TryAdd (handler);
    }

    public sealed override void Clear (IHandler entity)
    {
      base.Clear (entity);

      if (!Set.Remove (entity))
        KeySet.Remove (entity);
    }

    protected override void OnElementAdded (IHandler handler)
    {
      base.OnElementAdded (handler);

      GetSubscriber ().Subscribe (handler);
    }

    protected override void OnElementRemoved (IHandler handler)
    {
      base.OnElementRemoved (handler);

      GetSubscriber ().Unsubscribe (handler);
    }

    protected virtual void OnKeyAdded (Type key)
    {
      rootKeyBuilder?.OnAdded (key);
    }

    protected virtual void OnKeyRemoved (Type key)
    {
      rootKeyBuilder?.OnRemoved (key);
    }

    protected override void OnActivate ()
    {
      base.OnActivate ();

      GetSubscriber ().Activate ();
    }

    protected override void OnDeactivate ()
    {
      base.OnDeactivate ();

      GetSubscriber ().Deactivate ();
    }

    internal SubscriptionService GetSubscriber ()
      => subscriber ??= new SubscriptionService (Hub.Events);

    void IContainer<Type>.OnAdded (Type key) => OnKeyAdded (key);
    void IContainer<Type>.OnRemoved (Type key) => OnKeyRemoved (key);
    IContainer<Type> IContainer<Type>.RootContainer { get => rootKeyBuilder; set => rootKeyBuilder = value; }
  }
}