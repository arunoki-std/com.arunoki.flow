using Arunoki.Flow.Collections;

using System;

namespace Arunoki.Flow.Globals
{
  public class GlobalHub : FlowHub
  {
    public GlobalHub (IContext entityContext, bool autoActivate = false)
      : base (entityContext, false)
    {
      if (Instance != null)
        throw new InvalidOperationException (
          $"{nameof(GlobalHub)} already created. One instance per application.");

      Instance = this;
      Managers = new(this);

      if (autoActivate) Activate ();
    }

    public GlobalHub (IContext entityContext, RuntimeManagers runtimeManagers, bool autoActivate = true)
      : this (entityContext)
    {
      foreach (var staticType in runtimeManagers)
        Managers.Add (staticType);

      if (autoActivate) Activate ();
    }

    internal static GlobalHub Instance { get; private set; }

    public ManagersCollection Managers { get; }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      Managers.Initialize ();
    }
  }
}