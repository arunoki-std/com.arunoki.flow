namespace Arunoki.Flow.Sample
{
  public partial class FsmEntity : IContext
  {
    public Signal<TimeoutEvent> Timeout { get; } = new();

    public FsmEntity ()
    {
    }
  }
}