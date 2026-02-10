using Arunoki.Collections;
using Arunoki.Flow.Basics;

namespace Arunoki.Flow.Builders
{
  public class ServicesBuilder : HubBuilder<IService>
  {
    public ServicesBuilder (IContainer<IService> rootContainer = null) : base (rootContainer)
    {
    }

    public override bool IsConsumable (IService service)
    {
      switch (service)
      {
        case IBuilder:
        case IContext:
        case IPipeline:
        case IHandler:
          return false;

        default: return service is not null;
      }
    }
  }
}