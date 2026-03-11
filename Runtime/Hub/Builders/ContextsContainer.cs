using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;

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
      Composition = new ServiceWithElements<IContext> (GetAllElements ());
    }

    public IContext Root { get; }

    protected override void OnInit ()
    {
      foreach (IContext context in this)
        Hub.Register (context);

      base.OnInit ();
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

      if (context is IUpdatable updatable && updatable != Root)
        Hub.Updater.Set.TryAdd (updatable);
    }

    protected override void OnElementRemoved (IContext context)
    {
      base.OnElementRemoved (context);

      var contextType = context.GetType ();
      Hub.Events.UnregisterSource (context);
      Hub.Services.KeySet.Clear (contextType);

      if (context is IUpdatable updatable && updatable != Root)
        Hub.Updater.Set.Remove (updatable);
    }

    protected override bool CanBuildAfterHubInit () => false;
    protected override bool CanBuildAfterHubStarted () => false;
    protected override bool CanBuildAfterHubActivation () => false;
    protected override bool IsMultiInstancesSupported () => false;

    public override int GetBuildOrder () => (int) FlowHub.BuildOrder.Contexts;
  }
}