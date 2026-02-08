using Arunoki.Collections;
using Arunoki.Collections.Utilities;

namespace Arunoki.Flow.Collections
{
  public class ContextSet : BaseSet<IContext>
  {
    public ContextSet (IContext context, IContainer<IContext> rootContainer = null)
      : base (rootContainer)
    {
      (this as IContextPart).Set (context);

      TryAdd (context);
    }


    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      ForEach (context => Hub.Produce (context));
    }

    public virtual void Reset ()
    {
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

      AddRange (context.FindPropertiesWithNested<IContext> ().ToArray ());
    }

    protected override void OnElementRemoved (IContext context)
    {
      base.OnElementRemoved (context);

      Hub.Events.UnregisterSource (context);
    }
  }
}