using Arunoki.Collections;
using Arunoki.Collections.Utilities;

namespace Arunoki.Flow.Misc
{
  public partial class ContextSet : IService
  {
    private bool isInitialized;
    private SubscriptionService subscriber;

    protected SubscriptionService Subscriber => subscriber ??= new SubscriptionService (Hub.Events, false);

    public bool IsActive => subscriber != null && subscriber.IsActive;

    public void Activate ()
    {
      if (!isInitialized)
      {
        isInitialized = true;

        Cast<IHandler> (handler => Subscriber.Subscribe (handler));
      }

      Subscriber.Activate ();
    }

    public void Deactivate () => Subscriber.Deactivate ();
  }
}