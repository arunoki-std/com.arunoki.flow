using Arunoki.Collections;
using Arunoki.Flow.Sample.Handlers;
using Arunoki.Flow.Sample.Events;

using System;

namespace Arunoki.Flow.Sample
{
  public partial class Battery : IContext, IPipeline, IContainer<IHandler>, IDisposable
  {
    public readonly FlowHub Hub;
    private IContainer<IHandler> targetContainer;

    public FloatProperty<PowerEvent> Power { get; } = new();
    public BoolProperty<ChargeStatusEvent> IsCharged { get; } = new();
    public Trigger<OverloadEvent> Overload { get; } = new();

    public Battery ()
    {
      Hub = new(this);
      Hub.Pipeline.Produce<BatteryPipeline> ();
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
      Hub.Events.Reset ();
      // Power.Reset ();
      // IsCharged.Reset ();
      // Overload.Reset ();
    }

    IContainer<IHandler> IContainer<IHandler>.TargetContainer
    {
      get => targetContainer;
      set => targetContainer = value;
    }

    void IContainer<IHandler>.OnElementAdded (IHandler element)
    {
      UnityEngine.Debug.LogWarning ($"element added: {element}");
    }

    void IContainer<IHandler>.OnElementRemoved (IHandler element)
    {
      UnityEngine.Debug.LogWarning ($"element removed: {element}");
    }

    public void Dispose ()
    {
      Hub.Clear ();
    }
  }
}