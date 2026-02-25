namespace Arunoki.Flow.Basics
{
  public abstract class BaseHubContainer : HubPart
  {
    protected internal virtual int GetBuildOrder () => (int) FlowHub.BuildOrder.Any;
  }
}