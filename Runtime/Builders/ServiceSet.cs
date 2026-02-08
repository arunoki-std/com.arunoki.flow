using Arunoki.Collections;

namespace Arunoki.Flow.Collections
{
  public class ServiceSet : BaseSet<IService>, IService
  {
    public ServiceSet (IContainer<IService> rootContainer = null) : base (rootContainer)
    {
    }

    public bool IsActive { get; private set; }

    public void Activate ()
    {
      if (IsActive) return;

      IsActive = true;
      OnActivated ();
    }

    public void Deactivate ()
    {
      if (!IsActive) return;

      IsActive = false;
      OnDeactivated ();
    }

    protected virtual void OnActivated ()
    {
      foreach (var service in Elements)
        if (service is not IManuallyActivatedService)
          service.Activate ();
    }

    protected virtual void OnDeactivated ()
    {
      foreach (var service in Elements)
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