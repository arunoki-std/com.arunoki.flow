namespace Arunoki.Flow
{
    /// <summary>
    /// Marks a receiver that should be subscribed to a local flow hub.
    /// </summary>
    public interface IFlowHandler { }

    /// <summary>
    /// Marks a receiver that should be subscribed to the global flow hub
    /// by higher-level infrastructure.
    /// </summary>
    public interface IFlowGlobalHandler : IFlowHandler { }

    public interface IFlowConditionHandler : IFlowHandler
    {
        bool IsHandlingEvents { get; }
    }

    public interface IFlowServiceHandler : IFlowConditionHandler
    {
        void OnActivated();

        void OnDeactivated();
    }

    public interface IFlowResettableHandler : IFlowHandler
    {
        void OnReset();
    }
}
