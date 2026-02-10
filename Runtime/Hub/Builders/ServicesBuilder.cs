using Arunoki.Flow.Basics;

namespace Arunoki.Flow.Builders
{
  public class ServicesBuilder : HubBuilder<IService>
  {
    public override bool IsConsumable (IService service)
    {
      return service switch
      {
        IBuilder or IContext or IPipeline or IHandler => false,
        _ => service is not null
      };
    }
  }
}