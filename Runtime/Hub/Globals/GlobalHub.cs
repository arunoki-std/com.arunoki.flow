using Arunoki.Flow.Builders;

using System;

namespace Arunoki.Flow.Globals
{
  /// Do not use singleton instance in static constructors.
  public class GlobalHub : FlowHub
  {
    public event Action OnReady;
    private bool isReady;

    public GlobalHub (bool autoActivate = false)
      : this (new DummyContext (), autoActivate)
    {
    }

    public GlobalHub (IContext context, bool autoActivate = false) : base (context, false)
    {
      if (Instance != null)
        throw new InvalidOperationException ($"{nameof(GlobalHub)} already created. One instance per application.");

      Instance = this;
      Managers = new(this);

      if (autoActivate) Activate ();
    }

    public static GlobalHub Init (GlobalHub hub, StaticBootstrap bootstrap)
    {
      foreach (var staticType in bootstrap)
        hub.Managers.Produce (staticType);

      hub.Activate ();
      return hub;
    }

    public static bool IsAssemblyInitialized => Instance != null;

    public ManagersBuilder Managers { get; }

    internal static GlobalHub Instance { get; private set; }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      // Managers.Initialize ();
    }

    protected override void OnActivated ()
    {
      base.OnActivated ();

      if (!isReady)
      {
        isReady = true;
        OnReady?.Invoke ();
        OnReady = null;
      }
    }

    /// Remove all elements from hub components and collections. 
    public override void ClearAll ()
    {
      base.ClearAll ();

      Managers.ClearAll ();
    }
  }
}