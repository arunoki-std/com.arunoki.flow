using System;

namespace Arunoki.Flow
{
  public class MultiplePipelineHandlerRegistrationException : Exception
  {
    public readonly Type HandlerType;

    public MultiplePipelineHandlerRegistrationException (Type handlerType)
      : base ($"{nameof(IPipelineHandler)} '{handlerType}' has already been registered.")
    {
      HandlerType = handlerType;
    }
  }
}