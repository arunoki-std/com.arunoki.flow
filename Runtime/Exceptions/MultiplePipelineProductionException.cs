using System;

namespace Arunoki.Flow
{
  public class MultiplePipelineProductionException : Exception
  {
    public readonly Type PipelineType;

    public MultiplePipelineProductionException (Type pipelineType)
      : base ($"Pipeline '{pipelineType}' has already been instantiated.")
    {
      PipelineType = pipelineType;
    }
  }
}