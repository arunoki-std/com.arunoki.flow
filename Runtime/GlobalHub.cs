using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  public class GlobalHub : FlowHub
  {
    public static GlobalHub Instance { get; private set; }

    public GlobalHub (IContext entityContext) : base (entityContext, false)
    {
      if (Instance != null)
        throw new InvalidOperationException (
          $"{nameof(GlobalHub)} already created. Only one instance per application.");

      Instance = this;
      Managers = new(this);
    }

    public ManagersSet Managers { get; }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      Managers.TryInitialize ();
    }
  }
}