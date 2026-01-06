using System;

namespace Arunoki.Flow
{
  public class MultiplePipelineProduction : Exception
  {
    public readonly Type PipelineType;

    public MultiplePipelineProduction (Type pipelineType)
      : base ($"Pipeline '{pipelineType}' has already been instantiated.")
    {
      PipelineType = pipelineType;
    }
  }
}