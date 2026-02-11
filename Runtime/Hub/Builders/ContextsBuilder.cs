using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;

namespace Arunoki.Flow.Builders
{
  public class ContextsBuilder : HubBuilder<IContext>
  {
    public IContext Root { get; private set; }

    protected internal void SetRoot (IContext root)
    {
      Root = root;
      (this as IContextPart).Set (root);
      Set.TryAdd (root);
    }

    protected override void OnInitialized ()
    {
      foreach (IContext context in this)
        Hub.Produce (context);

      base.OnInitialized ();
    }

    protected override void OnElementAdded (IContext context)
    {
      base.OnElementAdded (context);

      Hub.Events.RegisterSource (context);

      if (context is IContextPart ctxPart && ctxPart.Get () == null)
        ctxPart.Set (Context);

      var allServices = context.FindProperties<IService> ();
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

    protected internal override int GetBuildOrder () => (int) FlowHub.BuildOrder.Contexts;
  }
}