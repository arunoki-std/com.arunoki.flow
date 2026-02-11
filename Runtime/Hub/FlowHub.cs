using Arunoki.Flow.Basics;
using Arunoki.Flow.Builders;
using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
  public partial class FlowHub : ServiceContainer<IBuilder>
  {
    public EventBus Events { get; } = new();
    public ContextsBuilder Contexts { get; } = new();
    public ServicesBuilder Services { get; } = new();
    public PipelineBuilder Pipeline { get; } = new();
    public HandlersBuilder Handlers { get; } = new();

    public FlowHub (IContext entityContext, bool autoInit = true)
    {
      OnCreateBuilders (entityContext);
      Contexts.SetRoot (entityContext);
      OnInitBuilders ();

      if (autoInit) Initialize ();
    }
  }
}