using Arunoki.Flow.Basics;
using Arunoki.Flow.Builders;
using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
  public partial class FlowHub : ServiceContainer<IBuilder>
  {
    public EventBus Events { get; } = new();
    public ContextsBuilder Contexts { get; }
    public ServicesBuilder Services { get; } = new();
    public PipelineBuilder Pipeline { get; } = new();
    public HandlersBuilder Handlers { get; } = new();

    public FlowHub (IContext entityContext, bool autoInit = true)
    {
      Contexts = new ContextsBuilder (entityContext);
      OnCreateBuilders (entityContext);
      OnInitBuilders ();
      Contexts.Set.TryAdd (entityContext);

      if (autoInit) Initialize ();
    }
  }
}