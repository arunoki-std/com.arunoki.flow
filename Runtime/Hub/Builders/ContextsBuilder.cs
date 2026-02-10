using Arunoki.Collections;
using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;

namespace Arunoki.Flow.Builders
{
  public class ContextsBuilder : HubBuilder<IContext>
  {
    public IContext Root { get; }

    public ContextsBuilder (IContext context, IContainer<IContext> rootContainer = null)
      : base (rootContainer)
    {
      Root = context;

      (this as IContextPart).Set (context);
      Elements.TryAdd (context);
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      foreach (IContext context in this)
        Hub.Produce (context);
    }

    protected override void OnReset ()
    {
      base.OnReset ();

      foreach (var resettable in Cast<IResettable> ())
        if (resettable.AutoReset ())
          resettable.Reset ();
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

      Elements.AddRange (context.FindPropertiesWithNested<IContext> ().ToArray ());
    }

    protected override void OnElementRemoved (IContext context)
    {
      base.OnElementRemoved (context);

      Hub.Events.UnregisterSource (context);
      Hub.Services.KeySet.Clear (context.GetType ());
    }

    protected virtual void OnServiceAdded (IService service)
    {
      (this as IContainer<IService>).RootContainer?.OnAdded (service);
    }

    protected virtual void OnServiceRemoved (IService service)
    {
      (this as IContainer<IService>).RootContainer?.OnRemoved (service);

      Hub.Services.Clear (service);
    }

    protected override bool CanBuildAfterHubInit () => false;
    protected override bool CanBuildAfterHubStarted () => false;
    protected override bool CanBuildAfterHubActivation () => false;
  }
}