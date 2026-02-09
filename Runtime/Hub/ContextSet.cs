using Arunoki.Collections;
using Arunoki.Collections.Utilities;

namespace Arunoki.Flow.Basics
{
  public class ContextSet : BaseHubCollection<IContext>
  {
    public ContextSet (IContext context, IContainer<IContext> rootContainer = null)
      : base (rootContainer)
    {
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

      foreach (var service in context.FindProperties<IService> ())
      {
        if (service is IContextPart part && part.Get () == null)
          part.Set (context);

        Hub.OnTryAddService (service);
      }

      Elements.AddRange (context.FindPropertiesWithNested<IContext> ().ToArray ());
    }

    protected override void OnElementRemoved (IContext context)
    {
      base.OnElementRemoved (context);

      Hub.Events.UnregisterSource (context);
    }

    protected override bool CanBuildAfterHubInit () => false;
    protected override bool CanBuildAfterHubStarted () => false;
    protected override bool CanBuildAfterHubActivation () => false;
  }
}