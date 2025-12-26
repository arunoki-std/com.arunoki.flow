namespace Arunoki.Flow
{
  public interface IEventsHubPart : IEventsContextPart
  {
    EventHub Hub { get; }

    void Init (EventHub hub);
  }
}