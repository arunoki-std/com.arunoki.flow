using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;

using System;
using System.Reflection;

namespace Arunoki.Flow.Builders
{
  public class ContextsContainer : HubContainer<IContext>
  {
    public ContextsContainer (IContext root, FlowHub hub)
    {
      Root = root;
      (this as IContextPart).Set (root);
      (this as IHubPart).Set (hub);
    }

    public IContext Root { get; }

    protected override void OnInitialized ()
    {
      foreach (IContext context in this)
        Hub.Register (context);

      base.OnInitialized ();
    }

    protected void InitServices ()
    {
      foreach (var context in this)
      {
        var allServices =
          context.FindProperties<IService> (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (allServices.Count > 0)
        {
          var set = Hub.Services.KeySet.GetOrCreate (context.GetType ());
          foreach (var service in allServices)
          {
            if (service is IContextPart part && part.Get () == null)
              part.Set (context);

            set.TryAdd (service);
          }
        }
      }
    }

    protected override void OnElementAdded (IContext context)
    {
      base.OnElementAdded (context);

      Hub.Events.RegisterSource (context);

      Set.AddRange (context.FindPropertiesWithNested<IContext> ().ToArray ());

      InitServices ();
    }

    protected override void OnElementRemoved (IContext context)
    {
      base.OnElementRemoved (context);

      var contextType = context.GetType ();
      Hub.Events.UnregisterSource (context);
      Hub.Services.KeySet.Clear (contextType);
    }

    protected override void OnLateActivate ()
    {
      base.OnLateActivate ();

      var list = GetAllElements ();
      for (var index = 0; index < list.Count; index++)
        if (list [index] is ILateService service)
          service.LateActivate ();
    }

    protected override bool CanBuildAfterHubInit () => false;
    protected override bool CanBuildAfterHubStarted () => false;
    protected override bool CanBuildAfterHubActivation () => false;
    protected override bool IsMultiInstancesSupported () => false;

    protected internal override int GetBuildOrder () => (int) FlowHub.BuildOrder.Contexts;
  }
}