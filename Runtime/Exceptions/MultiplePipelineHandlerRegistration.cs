using System;

namespace Arunoki.Flow
{
  public class MultiplePipelineHandlerRegistration : Exception
  {
    public readonly Type HandlerType;

    public MultiplePipelineHandlerRegistration (Type handlerType)
      : base ($"{nameof(IPipelineHandler)} '{handlerType}' has already been registered.")
    {
      HandlerType = handlerType;
    }
  }
}