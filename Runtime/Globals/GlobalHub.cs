using Arunoki.Flow.Collections;

using System;

namespace Arunoki.Flow.Globals
{
  public class GlobalHub : FlowHub
  {
    public GlobalHub (IContext context, bool autoActivate = false) : base (context, false)
    {
      if (Instance != null)
        throw new InvalidOperationException ($"{nameof(GlobalHub)} already created. One instance per application.");

      Instance = this;
      Managers = new(this);

      if (autoActivate) Activate ();
    }

    public GlobalHub (IContext context, StaticBootstrap bootstrap, bool autoActivate = true) : this (context)
    {
      foreach (var staticType in bootstrap)
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