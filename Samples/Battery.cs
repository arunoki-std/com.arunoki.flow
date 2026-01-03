using Arunoki.Flow.Samples.Events;

namespace Arunoki.Flow.Samples
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

    public void DoChargeTest ()
    {
      Power.Set (0.1f);
      Power.Set (0.6f);
      Power.Set (1.0f);
      Power.Set (1.1f);
    }

    private void Charged ()
    {
      IsCharged.Set (true);
    }

    private void NotCharged ()
    {
      IsCharged.Set (false);
    }
  }
}