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
    public ContextsBuilder Contexts { get; }
    public ServicesBuilder Services { get; } = new();
    public PipelineBuilder Pipeline { get; } = new();
    public HandlersBuilder Handlers { get; } = new();

    public FlowHub (IContext context, bool autoInit = true)
    {
      Contexts = new ContextsBuilder (context, this);

      FindPartsAt (this);
      FindPartsAt (context);

      if (autoInit) Initialize ();
    }

    protected virtual void FindPartsAt (object target)
    {
      if (target is IDummy) return;

      var prevCount = Elements.Count;

      foreach (var builder in target.FindProperties<IHubContainer> ())
      {
        TryInjectDependencies (builder);
        Elements.Add (builder);
      }

      if (Elements.Count != prevCount) SortBuilders ();
    }

    protected internal virtual void TryInjectDependencies (object entity)
    {
      if (entity is IHubPart hubPart && hubPart.Get () == null) hubPart.Set (this);
      if (entity is IContextPart ctxPart && ctxPart.Get () == null) ctxPart.Set (Contexts.Root);
    }

    protected void SortBuilders ()
      => Elements.Sort ((a, b) => Order (a).CompareTo (Order (b)));

    private static int Order (IHubContainer x) =>
      x is BaseHubBuilder bb ? bb.GetBuildOrder () : (int) FlowHub.BuildOrder.Any;
  }
}