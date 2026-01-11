using Arunoki.Collections;
using Arunoki.Collections.Utilities;

namespace Arunoki.Flow.Misc
{
  public class ContextSet : SetsTypeCollection<IContext>, IBuilder
  {
    protected readonly FlowHub Hub;

    protected ContextSet (FlowHub hub) => Hub = hub;

    public ContextSet (FlowHub hub, IContext context) : this (hub) => Produce (context);

    void IBuilder.Produce (object element)
    {
      switch (element)
      {
        case IContext ctx:
          Produce (ctx);
          break;
      }
    }

    public void Produce (IContext context)
    {
      var contextType = context.GetType ();

      Add (contextType, context);
      Add (contextType, context.GetAllPropertiesWithNested<IContext> ().ToArray ());
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

    public bool IsConsumable (object element)
      => element is IContext;
  }
}