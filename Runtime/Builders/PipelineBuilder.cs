using Arunoki.Collections;
using Arunoki.Collections.Utilities;

using System;

namespace Arunoki.Flow.Collections
{
  public class PipelineBuilder : BaseBuilder<IPipeline>
  {
    protected readonly Set<IPipeline> Pipelines;

    public PipelineBuilder (IContainer<IPipeline> rootContainer = null) : base (rootContainer)
    {
      Pipelines = new(this);
    }

    protected virtual HandlersBuilder Handlers => Hub.Handlers;

    public override void Produce (IPipeline pipeline)
    {
      base.Produce (pipeline);

      Pipelines.TryAdd (pipeline);
    }

    public void Produce<TPipeline> () where TPipeline : IPipeline, new ()
    {
      Produce (Activator.CreateInstance (typeof(TPipeline)) as IPipeline);
    }

    public override void Clear (IPipeline entity)
    {
      base.Clear (entity);

      Pipelines.Remove (entity);
    }

    public void Clear<TPipeline> () where TPipeline : IPipeline
    {
      Clear (typeof(TPipeline));
    }

    public void Clear (Type pipelineType)
    {
      Pipelines.RemoveWhere (p => p.GetType () == pipelineType);
    }

    /// Clear all pipelines and handlers.
    public void Clear ()
    {
      Pipelines.Clear ();
    }

    protected virtual void CreateHandlers (Type pipelineType, IContext context)
    {
      var set = Handlers.KeySet.GetOrCreate (pipelineType);
      var handlerTypes = pipelineType.GetNestedTypes<IHandler> ();

      for (var i = 0; i < handlerTypes.Count; i++)
      {
        var handler = (IHandler) Activator.CreateInstance (handlerTypes [i]);
        if (handler is IContextPart part && part.Get () == null) part.Set (context);
        set.TryAdd (handler);
      }
    }

    protected override void OnElementAdded (IPipeline pipeline)
    {
      base.OnElementAdded (pipeline);

      if (pipeline is IContextPart part && part.Get () == null) part.Set (Context);
      if (pipeline is IHubPart hubPart && hubPart.Get () == null) hubPart.Set (Hub);

      var context = pipeline as IContext ?? (pipeline is IContextPart p && p.Get () != null ? p.Get () : Context);

      CreateHandlers (pipeline.GetType (), context);
    }

    protected override void OnElementRemoved (IPipeline pipeline)
    {
      base.OnElementRemoved (pipeline);

      if (pipeline is IContextPart cxtPart) cxtPart.Set (null);
      if (pipeline is IHubPart hubPart) hubPart.Set (null);

      Handlers.KeySet.Clear (pipeline.GetType ());
    }
  }
}