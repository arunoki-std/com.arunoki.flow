using Arunoki.Collections;
using Arunoki.Flow.Events.Core;

using System;

namespace Arunoki.Flow.Basics
{
  public class HandlersBuilder : BaseHubCollection<IHandler>, IContainer<Type>
  {
    protected internal readonly SetsTypeCollection<IHandler> KeySet;

    private IContainer<Type> rootKeyBuilder;

    private SubscriptionService subscriber;

    public HandlersBuilder () : this (null, null) { }

    public HandlersBuilder (IContainer<Type> rootKeyBuilder) : this (null, rootKeyBuilder) { }

    public HandlersBuilder (IContainer<IHandler> rootBuilder, IContainer<Type> rootKeyBuilder = null)
      : base (rootBuilder)
    {
      this.rootKeyBuilder = rootKeyBuilder;

      KeySet = new(this, this);
    }

    /// Encapsulates Events (Subscribe / Unsubscribe) without Handlers allocation when Hub (Activated / Deactivated).
    internal SubscriptionService Subscriber => (subscriber ??= new SubscriptionService (Hub.Events));

    public sealed override bool Clear (IHandler entity)
    {
      return base.Clear (entity) || KeySet.Remove (entity);
    }

    protected override void OnElementAdded (IHandler handler)
    {
      base.OnElementAdded (handler);

      Subscriber.Register (handler);
    }

    protected override void OnElementRemoved (IHandler handler)
    {
      base.OnElementRemoved (handler);

      Subscriber.Remove (handler);
    }

    protected virtual void OnKeyAdded (Type key)
    {
      rootKeyBuilder?.OnAdded (key);
    }

    protected virtual void OnKeyRemoved (Type key)
    {
      rootKeyBuilder?.OnRemoved (key);
    }

    protected override void OnActivated ()
    {
      base.OnActivated ();

      Subscriber.Activate ();
    }

    protected override void OnDeactivated ()
    {
      base.OnDeactivated ();

      Subscriber.Deactivate ();
    }

    void IContainer<Type>.OnAdded (Type key) => OnKeyAdded (key);
    void IContainer<Type>.OnRemoved (Type key) => OnKeyRemoved (key);
    IContainer<Type> IContainer<Type>.RootContainer { get => rootKeyBuilder; set => rootKeyBuilder = value; }

    protected override bool IsMultiInstancesSupported () => false;
  }
}