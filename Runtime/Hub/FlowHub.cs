using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;
using Arunoki.Flow.Builders;
using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
  public partial class FlowHub : ServiceContainer<IHubContainer>
  {
    internal enum BuildOrder
    {
      Any = 0,
      Managers = short.MinValue,
      Contexts = short.MinValue + 1,
      Services = short.MinValue + 2,
      Pipelines = short.MinValue + 3,
      Handlers = short.MinValue + 4
    }

    public EventBus Events { get; } = new();
    public ContextsContainer Contexts { get; }
    public ServicesContainer Services { get; } = new();
    public PipelineContainer Pipeline { get; } = new();
    public HandlersContainer Handlers { get; } = new();

    public FlowHub (IContext context, bool autoInit = true)
    {
      Contexts = new ContextsContainer (context, this);

      FindPartsAt (this);
      FindPartsAt (context);

      if (autoInit) Initialize ();
    }

    protected virtual void FindPartsAt (object target)
    {
      if (target is IDummy) return;

      var prevCount = Elements.Count;

      foreach (var container in target.FindProperties<IHubContainer> ())
      {
        TryInjectDependencies (container);
        Elements.Add (container);
      }

      if (Elements.Count != prevCount) SortContainers ();
    }

    protected internal virtual void TryInjectDependencies (object entity)
    {
      if (entity is IHubPart hubPart && hubPart.Get () == null) hubPart.Set (this);
      if (entity is IContextPart ctxPart && ctxPart.Get () == null) ctxPart.Set (Contexts.Root);
    }

    protected void SortContainers ()
      => Elements.Sort ((a, b) => Order (a).CompareTo (Order (b)));

    private static int Order (IHubContainer x) =>
      x is BaseHubContainer bb ? bb.GetBuildOrder () : (int) FlowHub.BuildOrder.Any;
  }
}