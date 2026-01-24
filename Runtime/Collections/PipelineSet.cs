using Arunoki.Collections;
using Arunoki.Collections.Utilities;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Misc
{
  /// Обрабатывает объекты <see cref="IPipeline"/>, <see cref="IPipelineHandler"/>.
  public class PipelineSet : BaseHandlerSet
  {
    protected readonly SetsTypeCollection<IHandler> Handlers;
    protected readonly List<IPipeline> Pipelines = new(4);

    public PipelineSet ()
    {
      Handlers = new(this);
    }

    protected override void Produce (object element)
    {
      switch (element)
      {
        case IPipeline pipeline:
          Produce (pipeline);
          break;

        case IPipelineHandler handler:
          Produce (handler);
          break;
      }
    }

    public void Produce<TPipeline> () where TPipeline : IPipeline, new ()
    {
      Produce (Activator.CreateInstance (typeof(TPipeline)) as IPipeline);
    }

    public void Produce (IPipeline pipeline)
    {
      if (Utils.IsDebug ())
      {
        var type = pipeline.GetType ();

        if (Pipelines.FindIndex (e => e.GetType () == type) > -1)
          throw new MultiplePipelineProductionException (type);
      }

      Pipelines.Add (pipeline);
      OnPipelineAdded (pipeline);

      var context = pipeline is IContext ctx ? ctx : Context;
      var pipelineType = pipeline.GetType ();

      ProducePipelineHandlers (pipelineType, context);

      if (pipeline is IPipelineHandler handler)
        ProduceHandler (handler, pipelineType, context);
    }

    public void Produce (IPipelineHandler handler)
    {
      var ctx = handler is IContext selfContext ? selfContext : Context;
      var ppl = handler is IPipeline selfPipeline ? selfPipeline : null;
      var pipelineType = ppl != null ? ppl.GetType () : ctx.GetType ();

      ProduceHandler (handler, pipelineType, ctx);
    }

    public void Remove<TPipeline> () where TPipeline : IPipelineHandler
    {
      RemovePipeline (typeof(TPipeline));
    }

    public void RemovePipeline (Type pipelineType)
    {
      Handlers.Clear (pipelineType);

      var index = Pipelines.FindIndex (pipeline => pipeline.GetType () == pipelineType);
      if (index > -1)
      {
        var pipeline = Pipelines [index];
        Pipelines.RemoveAt (index);
        OnPipelineRemoved (pipeline);
      }
    }

    public void Remove (IPipelineHandler handler)
    {
      Handlers.Remove (handler);
    }

    /// Clear all pipelines and handlers.
    public override void Clear ()
    {
      base.Clear ();

      for (var index = Pipelines.Count - 1; index >= 0; index--)
      {
        var pipeline = Pipelines [index];
        Pipelines.RemoveAt (index);
        OnPipelineRemoved (pipeline);
      }
    }

    protected virtual void ProduceHandler (IPipelineHandler handler, Type pipelineType, IContext context)
    {
      ProduceHandler (handler, Handlers.GetOrCreate (pipelineType), context);
    }

    /// <exception cref="MissingConstructorException"></exception>
    protected virtual void ProducePipelineHandlers (Type pipelineType, IContext context)
    {
      var set = Handlers.GetOrCreate (pipelineType);
      var list = pipelineType.GetNestedTypes<IPipelineHandler> ();

      for (var i = 0; i < list.Count; i++)
      {
        try
        {
          var handler = (IPipelineHandler) Activator.CreateInstance (list [i]);
          ProduceHandler (handler, set, context);
        }
        catch (MissingMethodException) { throw new MissingConstructorException (list [i].Name); }
      }
    }

    /// <exception cref="MultiplePipelineHandlerRegistrationException"></exception>
    protected virtual void ProduceHandler (IPipelineHandler handler, Set<IHandler> set, IContext context)
    {
      if (Utils.IsDebug ())
      {
        var handlerType = handler.GetType ();

        if (set.Any (e => handlerType == e.GetType ()))
          throw new MultiplePipelineHandlerRegistrationException (handlerType);
      }

      set.Add (handler);
    }

    /// To override.
    protected virtual void OnPipelineAdded (IPipeline pipeline)
    {
      if (pipeline is IContextPart part && part.Get () == null) part.Set (Context);
      if (pipeline is IHubPart hubPart && hubPart.Get () == null) hubPart.Set (Hub);
    }

    /// To override.
    protected virtual void OnPipelineRemoved (IPipeline pipeline)
    {
      if (pipeline is IContextPart part) part.Set (null);
      if (pipeline is IHubPart hubPart) hubPart.Set (null);
    }

    protected override Collections.ISet<IHandler> GetSet () => Handlers;

    public override bool IsConsumable (object element)
      => element is IPipeline || element is IPipelineHandler;
  }
}