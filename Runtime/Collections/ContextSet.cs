using Arunoki.Collections;
using Arunoki.Collections.Utilities;

namespace Arunoki.Flow.Misc
{
  public partial class ContextSet : BaseHubCollection<IContext>
  {
    private readonly Set<IContext> set;

    protected ContextSet (FlowHub hub)
    {
      set = new Set<IContext> (this);

      (this as IHubPart).Set (hub);
    }

    public ContextSet (FlowHub hub, IContext context) : this (hub)
    {
      (this as IContextPart).Set (context);

      Produce (context);
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      ForEach (context => Hub.Produce (context));
    }

    protected override void Produce (object element)
    {
      Produce (element as IContext);
    }

    public void Produce (IContext context)
    {
      set.Add (context);
      set.AddRange (context.GetAllPropertiesWithNested<IContext> ().ToArray ());
    }

    public void Remove (IContext context)
    {
      set.Remove (context);
    }

    protected override void OnElementAdded (IContext context)
    {
      base.OnElementAdded (context);

      Hub.Events.AddEventSource (context);
    }

    protected override void OnElementRemoved (IContext context)
    {
      base.OnElementRemoved (context);

      Hub.Events.RemoveEvents (context);
    }

    protected override ISet<IContext> GetSet () => set;

    public override bool IsConsumable (object element)
      => element is IContext;
  }
}