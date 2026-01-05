using Arunoki.Collections;
using Arunoki.Flow.Sample.Handlers;
using Arunoki.Flow.Sample.Events;

using System;

namespace Arunoki.Flow.Sample
{
  public partial class Battery : IContext, IPipeline, IContainer<IHandler>, IDisposable
  {
    private readonly FlowHub hub = new();
    private IContainer<IHandler> targetContainer;

    public Float<PowerEvent> Power { get; } = new();
    public Bool<ChargeStatusEvent> IsCharged { get; } = new();
    public EventChannel<OverloadEvent> Overload { get; } = new();

    public Battery ()
    {
      hub.Init (this);
      hub.Pipeline.Add<BatteryPipeline> ();
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
      hub.Clear ();
    }
  }
}