namespace Arunoki.Flow.Basics
{
  public abstract class BaseHubBuilder : HubPart
  {
    protected internal virtual int GetBuildOrder () => (int) FlowHub.BuildOrder.Any;
  }
}