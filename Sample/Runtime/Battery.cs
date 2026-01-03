using Arunoki.Flow.Sample.Events;

namespace Arunoki.Flow.Sample
{
  public partial class Battery : IContext, IPipeline
  {
    private readonly FlowHub hub = new();

    public Float<PowerEvent> Power { get; } = new();
    public Bool<ChargeStatusEvent> IsCharged { get; } = new();
    public EventChannel<OverloadEvent> Overload { get; } = new();

    public Battery ()
    {
      hub.Init (this);
    }

    private void Charged ()
    {
      IsCharged.Set (true);
    }

    private void NotCharged ()
    {
      IsCharged.Set (false);
    }

    public void Reset ()
    {
      Power.Reset ();
      IsCharged.Reset ();
    }
  }
}