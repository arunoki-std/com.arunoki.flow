using Arunoki.Flow.Basics;

namespace Arunoki.Flow.Builders
{
  public class ServicesContainer : HubContainer<IService>
  {
    public ServicesContainer ()
    {
      TargetService = new ServiceContainer<IService> (GetAllElements ());
    }

    protected override void OnLateActivate ()
    {
      base.OnLateActivate();
      
      var list = GetAllElements ();
      for (var index = 0; index < list.Count; index++)
        if (list [index] is ILateService service)
          service.LateActivate ();
    }

    protected override void OnElementAdded (IService element)
    {
      base.OnElementAdded (element);

      if (IsInitialized ())
      {
        if (element is IInitializable initializable && !initializable.IsInitialized ())
          initializable.Initialize ();

        if (element is IResettable resettable && resettable.AutoReset ())
          resettable.Reset ();
      }

      if (IsStarted ())
      {
        element.Activate ();
      }
    }

    public override bool IsConsumable (IService service)
    {
      return service switch
      {
        IHubContainer or IContext or IDummy => false,
        _ => service is not null
      };
    }

    protected internal override int GetBuildOrder () => (int) FlowHub.BuildOrder.Services;
  }
}