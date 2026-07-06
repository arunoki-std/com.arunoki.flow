namespace Arunoki.Flow.Sample
{
    public abstract class PipelineHandler : IFlowHandler, IContextPart
    {
        public SampleContext Context { get; private set; }

        IFlowContext IContextPart.Get() => Context;

        void IContextPart.Set(IFlowContext context) => Context = (SampleContext)context;
    }
}
