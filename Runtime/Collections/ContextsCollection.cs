using Arunoki.Collections;
using Arunoki.Collections.Utilities;

namespace Arunoki.Flow.Collections
{
  public class ContextsCollection : BaseHubCollection<IContext>
  {
    private readonly Set<IContext> set;

    protected ContextsCollection (FlowHub hub)
    {
      set = new Set<IContext> (this);

      (this as IHubPart).Set (hub);
    }

    public ContextsCollection (FlowHub hub, IContext context) : this (hub)
    {
      (this as IContextPart).Set (context);

      Add (context);
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      ForEach (context => Hub.Produce (context));
    }

    public void Add (IContext context)
    {
      set.Add (context);
      set.AddRange (context.GetAllPropertiesWithNested<IContext> ().ToArray ());
    }

    public void Remove (IContext context)
    {
      set.Remove (context);
    }

    public virtual void Reset ()
    {
      foreach (var resettable in set.Cast<IResettable> ())
        if (resettable.AutoReset ())
          resettable.Reset ();
    }

    protected override void OnElementAdded (IContext context)
    {
      base.OnElementAdded (context);

      Hub.Events.RegisterSource (context);
    }

    protected override void OnElementRemoved (IContext context)
    {
      base.OnElementRemoved (context);

      Hub.Events.UnregisterSource (context);
    }

    protected override ISet<IContext> GetSet () => set;
  }
}