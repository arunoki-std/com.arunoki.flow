namespace Arunoki.Flow.Misc
{
  public abstract class BaseHandlerSet : BaseHubCollectionService<IHandler>
  {
    private SubscriptionService subscriber;

    protected virtual SubscriptionService GetSubscriber ()
      => subscriber ??= new SubscriptionService (Hub.Events);

    protected override void OnElementAdded (IHandler element)
    {
      base.OnElementAdded (element);

      GetSubscriber ().Subscribe (element);
    }

    protected override void OnElementRemoved (IHandler element)
    {
      base.OnElementRemoved (element);

      GetSubscriber ().Unsubscribe (element);
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
  }
}