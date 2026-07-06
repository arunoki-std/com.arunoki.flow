namespace Arunoki.Flow
{
    public interface IFlowContext { }

    public interface IFlowHubContext : IFlowContext
    {
        FlowHub GetHub();
    }
}
