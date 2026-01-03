namespace Arunoki.Flow
{
  public interface IHubPart : IContextPart
  {
    FlowHub Hub { get; }

    void Init (FlowHub hub);
  }
}