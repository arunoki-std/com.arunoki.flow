using Arunoki.Flow.Builders;

using System;

using UnityEngine;

namespace Arunoki.Flow.Globals
{
  public class GlobalHub : FlowHub
  {
    /// Invoked once after hub activated.
    public static event Action OnReady = delegate { };

    private static bool _isReady;

    public GameObject View { get; internal set; }
    public ManagersContainer Managers { get; }

    /// Invoke from static constructors inside <see cref="OnReady"/> delegate.
    public static GlobalHub Instance { get; private set; }

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

      View = new GameObject ("Main.Flow");
      View.AddComponent<UpdateController> ();

      InitParts ();

      if (autoActivate) Activate ();
    }

    public static GlobalHub Init (GlobalHub hub, StaticBootstrap bootstrap)
    {
      foreach (var staticType in bootstrap)
        hub.Managers.Register (staticType);

      hub.Activate ();
      return hub;
    }

    public static bool IsAssemblyInitialized => Instance != null;

    protected override void OnActivate ()
    {
      base.OnActivate ();

      if (!_isReady)
      {
        _isReady = true;
        OnReady?.Invoke ();
        OnReady = null;
      }
    }

    private class DummyContext : IContext, IDummy
    {
    }
  }
}