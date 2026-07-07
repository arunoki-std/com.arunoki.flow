namespace Arunoki.Flow
{
    /// This node tells <see cref="FlowHub"/> that its nested classes contains <see cref="IFlowHandler"/> implementation.
    public interface IFlowPipeline { }

    public interface IActiveFlowPipeline : IFlowPipeline
    {
        bool IsActivated { get; }
        void OnActivated();
        void OnDeactivated();
    }

    public interface ILateFlowPipeline : IFlowPipeline
    {
        void OnLateActivate();
    }

    public interface IResettableFlowPipeline : IFlowPipeline
    {
        void OnReset();
    }
}
