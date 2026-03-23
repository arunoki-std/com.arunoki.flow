using Arunoki.Flow.Basics;
using Arunoki.Flow.Builders;
using Arunoki.Flow.Globals;

using System;

using UnityEngine;

namespace Arunoki.Flow
{
  public class GlobalHub : FlowHub
  {
    /// Invoked once after hub activated.
    public static event Action OnReady = delegate { };

    private static bool _isReady;

    private RoutineHelper routine;

    public ManagersContainer Managers { get; }

    /// Invoke from static constructors inside <see cref="OnReady"/> delegate.
    public static GlobalHub Instance { get; private set; }

    public GlobalHub (bool autoActivate = false)
      : this (new DummyContext (), autoActivate)
    {
    }

    public GlobalHub (IContext context, bool autoActivate = false)
      : base (context, false)
    {
      if (Instance != null)
        throw new InvalidOperationException ($"{nameof(GlobalHub)} already created. One instance per application.");

      Instance = this;
      Managers = new(this);

      InitParts ();
      InitRoutine ();

      if (autoActivate) Activate ();
    }

    private void InitRoutine ()
    {
      var gameObj = new GameObject ("Main.Flow") { hideFlags = HideFlags.NotEditable };
      UnityEngine.Object.DontDestroyOnLoad (gameObj);

      routine = gameObj.AddComponent<RoutineHelper> ();
      routine.OnFrameUpdate += Updater.Update;
      routine.OnLateUpdate += Updater.LateUpdate;
      routine.OnFixedUpdate += Updater.FixedUpdate;
    }

    public static GlobalHub Init (GlobalHub hub, StaticBootstrap bootstrap)
    {
      foreach (var staticType in bootstrap)
        hub.Managers.Register (staticType);

      hub.Activate ();
      return hub;
    }

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

    public RoutineHelper GetRoutine () => routine;

    public static bool IsAssemblyInitialized => Instance != null;

    private class DummyContext : IContext, IDummy
    {
    }
  }
}