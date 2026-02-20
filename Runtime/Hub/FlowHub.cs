using Arunoki.Flow.Basics;
using Arunoki.Flow.Builders;
using Arunoki.Flow.Events;
using Arunoki.Flow.Globals;

namespace Arunoki.Flow
{
  public partial class FlowHub : ServiceContainer<IHubBuilder>
  {
    public EventBus Events { get; } = new();
    public ContextsBuilder Contexts { get; }
    public ServicesBuilder Services { get; } = new();
    public PipelineBuilder Pipeline { get; } = new();
    public HandlersBuilder Handlers { get; } = new();

    public FlowHub (IContext context, bool autoInit = true)
    {
      Contexts = new ContextsBuilder (context, this);

      FindBuildersAt (this);
      if (context is not DummyContext) FindBuildersAt (context);
      OnInitBuilders ();

      if (autoInit) Initialize ();
    }
  }
}