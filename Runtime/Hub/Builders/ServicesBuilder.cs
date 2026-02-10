using Arunoki.Collections;
using Arunoki.Flow.Basics;

namespace Arunoki.Flow.Builders
{
  public class ServicesBuilder : HubBuilder<IService>
  {
    public ServicesBuilder (IContainer<IService> rootContainer = null) : base (rootContainer)
    {
    }

    protected override void OnActivated ()
    {
      base.OnActivated ();

      foreach (IService service in this)
        if (service is not IManuallyActivatedService)
          service.Activate ();
    }

    protected override void OnDeactivated ()
    {
      base.OnDeactivated ();

      foreach (IService service in this)
        service.Deactivate ();
    }

    public override bool IsConsumable (IService service)
    {
      switch (service)
      {
        case IBuilder:
        case IContext:
          return false;

        default: return service is not null;
      }
    }
  }
}