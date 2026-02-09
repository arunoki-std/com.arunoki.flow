using Arunoki.Collections;

namespace Arunoki.Flow.Basics
{
  public class ServiceSet : BaseHubCollection<IService>
  {
    public ServiceSet (IContainer<IService> rootContainer = null) : base (rootContainer)
    {
    }

    protected override void OnActivated ()
    {
      base.OnActivated ();

      foreach (IService service in Elements)
        if (service is not IManuallyActivatedService)
          service.Activate ();
    }

    protected override void OnDeactivated ()
    {
      base.OnDeactivated ();

      foreach (IService service in Elements)
        service.Deactivate ();
    }

    public override bool IsConsumable (IService service)
    {
      if (service is null) return false;
      if (service is IContext) return false;
      if (service is FlowHub) return false;

      return true;
    }
  }
}