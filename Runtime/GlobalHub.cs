using Arunoki.Flow.Misc;

using System;

namespace Arunoki.Flow
{
  public class GlobalHub : FlowHub
  {
    internal static GlobalHub Instance { get; private set; }

    public GlobalHub (IContext entityContext) : base (entityContext, false)
    {
      if (Instance != null)
        throw new InvalidOperationException (
          $"{nameof(GlobalHub)} already created. One instance per application.");

      Instance = this;
      Managers = new(this);
    }

    public ManagersCollection Managers { get; }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      Managers.Initialize ();
    }
  }
}