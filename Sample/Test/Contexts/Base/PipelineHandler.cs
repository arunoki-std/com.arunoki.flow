namespace Arunoki.Flow.Sample
{
  public abstract class PipelineHandler : IPipelineHandler, IContextPart
  {
    public SampleContext Context { get; private set; }

    IContext IContextPart.Get () => Context;

    void IContextPart.Set (IContext context) => Context = (SampleContext) context;
  }
}