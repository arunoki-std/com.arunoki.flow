using Arunoki.Flow.Basics;
using Arunoki.Flow.Events.Core;

namespace Arunoki.Flow.Builders
{
  public class HandlersContainer : HubContainer<IHandler>
  {
    private SubscriptionService subscriber;

    /// Encapsulates Events (Subscribe / Unsubscribe) without Handlers allocation when Hub (Activated / Deactivated).
    internal SubscriptionService Subscriber => (subscriber ??= new SubscriptionService (Hub.Events));

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

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      var list = GetAllElements ();
      for (var index = 0; index < list.Count; index++)
        if (list [index] is IInitializable initializable && !initializable.IsInitialized ())
          initializable.Initialize ();
    }

    protected override void OnReset ()
    {
      base.OnReset ();

      var list = GetAllElements ();
      for (var index = 0; index < list.Count; index++)
        if (list [index] is IResettableHandler handler)
          handler.OnReset ();
    }

    protected override void OnActivate ()
    {
      base.OnActivate ();

      Subscriber.Activate ();

      var list = GetAllElements ();
      for (var index = 0; index < list.Count; index++)
        if (list [index] is IActiveHandler handler)
          handler.OnActivated ();
    }

    protected override void OnLateActivate ()
    {
      base.OnLateActivate ();

      var list = GetAllElements ();
      for (var index = 0; index < list.Count; index++)
        if (list [index] is ILateHandler handler)
          handler.OnLateActivate ();
    }

    protected override void OnDeactivate ()
    {
      base.OnDeactivate ();

      Subscriber.Deactivate ();

      var list = GetAllElements ();
      for (var index = 0; index < list.Count; index++)
        if (list [index] is IActiveHandler handler)
          handler.OnDeactivated ();
    }

    protected override bool IsMultiInstancesSupported () => false;
    protected internal override int GetBuildOrder () => (int) FlowHub.BuildOrder.Handlers;
  }
}