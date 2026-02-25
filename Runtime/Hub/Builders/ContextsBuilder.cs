using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;

using System.Reflection;

namespace Arunoki.Flow.Builders
{
  public class ContextsBuilder : HubBuilder<IContext>
  {
    public ContextsBuilder (IContext root, FlowHub hub)
    {
      Root = root;
      (this as IContextPart).Set (root);
      (this as IHubPart).Set (hub);

      Set.TryAdd (root);
    }

    public IContext Root { get; }

    protected override void OnInitialized ()
    {
      foreach (IContext context in this)
        Hub.Register (context);

      base.OnInitialized ();
    }

    protected override void OnElementAdded (IContext context)
    {
      base.OnElementAdded (context);

      Hub.Events.RegisterSource (context);

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

      Set.AddRange (context.FindPropertiesWithNested<IContext> ().ToArray ());
    }

    protected override void OnElementRemoved (IContext context)
    {
      base.OnElementRemoved (context);

      Hub.Events.UnregisterSource (context);
      Hub.Services.KeySet.Clear (context.GetType ());
    }

    protected override bool CanBuildAfterHubInit () => false;
    protected override bool CanBuildAfterHubStarted () => false;
    protected override bool CanBuildAfterHubActivation () => false;
    protected override bool IsMultiInstancesSupported () => false;

    protected internal override int GetBuildOrder () => (int) FlowHub.BuildOrder.Contexts;
  }
}