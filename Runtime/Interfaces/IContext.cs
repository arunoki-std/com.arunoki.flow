namespace Arunoki.Flow
{
  public interface IContext
  {
  }

  public interface IFlowContext : IContext
  {
    FlowHub GetHub ();
  }
}