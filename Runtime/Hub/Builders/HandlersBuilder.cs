using Arunoki.Flow.Basics;
using Arunoki.Flow.Events.Core;

namespace Arunoki.Flow.Builders
{
  public class HandlersBuilder : HubBuilder<IHandler>
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

    protected override bool IsMultiInstancesSupported () => false;
    protected internal override int GetBuildOrder () => (int) FlowHub.BuildOrder.Handlers;
  }
}