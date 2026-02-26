using Arunoki.Flow.Basics;

namespace Arunoki.Flow.Builders
{
  public class ServicesContainer : HubContainer<IService>
  {
    public ServicesContainer ()
    {
      Composition = new ServiceWithElements<IService> (GetAllElements ());
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