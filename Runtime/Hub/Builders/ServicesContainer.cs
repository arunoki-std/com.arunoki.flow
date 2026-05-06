using Arunoki.Flow.Basics;

namespace Arunoki.Flow.Builders
{
  public class ServicesContainer : HubContainer<IService>
  {
    public ServicesContainer ()
    {
      Composition = new ServiceWithElements<IService> (GetAllElements ());
    }

    protected override void OnElementAdded (IService element)
    {
      base.OnElementAdded (element);

      if (element is IUpdatable updatable)
        Hub.Updater.Set.TryAdd (updatable);
    }

    protected override void OnElementRemoved (IService element)
    {
      base.OnElementRemoved (element);

      if (element is IUpdatable updatable)
        Hub.Updater.Set.Remove (updatable);
    }

    public override bool IsConsumable (IService service)
    {
      return service switch
      {
        IHubContainer or IFlowContext or IDummy => false,
        _ => service is not null
      };
    }

    public override int GetBuildOrder () => (int) FlowHub.BuildOrder.Services;
  }
}