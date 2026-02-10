using Arunoki.Collections;
using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;
using Arunoki.Flow.Builders;
using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
  public partial class FlowHub : BaseService
  {
    public EventBus Events { get; } = new();
    public ContextsBuilder Contexts { get; } = new();
    public ServicesBuilder Services { get; } = new();
    public PipelineBuilder Pipeline { get; } = new();
    public HandlersBuilder Handlers { get; } = new();

    public FlowHub (IContext entityContext, bool autoInit = true)
    {
      Contexts.SetRoot (entityContext);

      OnInitSets ();
      OnInitServices ();

      // ForEachSet<IHubPart> (part => part.Set (this));
      // ForEachSet<IContextPart> (part => part.Set (EntityContext));

      if (autoInit) Initialize ();
    }

    protected virtual IContainer<T> TryGetContainer<T> ()
    {
      return this as IContainer<T>;
    }

    protected virtual void OnInitSets ()
    {
      // AddSetsFrom (this);
      // AddSetsFrom (EntityContext);
    }

    protected virtual void OnInitServices ()
    {
      this.FindProperties<IService> (OnTryAddService);
    }

    protected internal virtual bool OnTryAddService (IService service)
    {
      if (Services.Set.TryAdd (service))
      {
        TryInjectDependencies (service);
        return true;
      }

      return false;
    }

    // protected override void OnSetAdded (ISet<IHandler> set)
    // {
    //   base.OnSetAdded (set);
    //
    //   if (set is IService service) OnTryAddService (service);
    // }
    //
    // protected override void OnElementAdded (IHandler element)
    // {
    //   base.OnElementAdded (element);
    //   TryInjectDependencies (element);
    // }
    //
    // protected override void OnElementRemoved (IHandler element)
    // {
    //   base.OnElementRemoved (element);
    //
    //   if (element is IDisposable disposable) disposable.Dispose ();
    //   if (element is IContextPart ctxPart) ctxPart.Set (null);
    //   if (element is IHubPart hubPart) hubPart.Set (null);
    // }

    // Remove all elements from hub components and collections.
    // public override void Clear ()
    // {
    //   Events.Clear ();
    //   Contexts.Elements.Clear ();
    //
    //   base.Clear ();
    // }

    /// Reset all mapped contexts and their event sources (properties, proxy data, triggers ets.) if they are
    /// <see cref="IResettable"/> and <see cref="IResettable.AutoReset"/> enabled. 
    protected override void OnReset ()
    {
      base.OnReset ();

      Events.Reset ();
      // Contexts.Reset ();
    }
  }
}