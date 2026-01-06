using Arunoki.Collections;
using Arunoki.Collections.Utilities;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow.Misc
{
  public class PipelineSet : HandlerSet
  {
    protected readonly SetsTypeCollection<IHandler> Handlers;
    protected readonly List<IPipeline> Pipelines = new(4);

    public PipelineSet ()
    {
      Handlers = new(this);
    }

    public override void Produce (object element)
    {
      if (element is IPipeline pipeline)
      {
        Produce (pipeline);
      }
      else if (element is IPipelineHandler handler)
      {
        Produce (handler);
      }
    }

    public virtual void Produce<TPipeline> () where TPipeline : IPipeline, new ()
    {
      Produce (Activator.CreateInstance (typeof(TPipeline)) as IPipeline);
    }

    public virtual void Produce (IPipeline pipeline)
    {
      if (Utils.IsDebug ())
      {
        var type = pipeline.GetType ();

        if (Pipelines.FindIndex (e => e.GetType () == type) > -1)
          throw new MultiplePipelineProduction (type);
      }

      Pipelines.Add (pipeline);

      if (pipeline is IContextPart part) part.Set (Context);
      if (pipeline is IHubPart hubPart) hubPart.Set (Hub);

      var context = pipeline is IContext ctx ? ctx : Context;
      var pipelineType = pipeline.GetType ();

      ProducePipelineHandlers (pipelineType, context);

      if (pipeline is IPipelineHandler handler)
        ProduceHandler (handler, pipelineType, context);
    }

    public virtual void Produce (IPipelineHandler handler)
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
      if (index > -1) Pipelines.RemoveAt (index);
    }

    public void Remove (IPipelineHandler handler)
    {
      Handlers.Remove (handler);
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

    /// <exception cref="MultiplePipelineHandlerRegistration"></exception>
    protected virtual void ProduceHandler (IPipelineHandler handler, Set<IHandler> set, IContext context)
    {
      if (Utils.IsDebug ())
      {
        var handlerType = handler.GetType ();

        if (set.Any (e => handlerType == e.GetType ()))
          throw new MultiplePipelineHandlerRegistration (handlerType);
      }

      if (handler is IContextPart part) part.Set (context);
      set.Add (handler);
    }

    protected override void OnElementAdded (IHandler element)
    {
      base.OnElementAdded (element);

      Hub.Events.Subscribe (element);
    }

    protected override void OnElementRemoved (IHandler element)
    {
      base.OnElementRemoved (element);

      Hub.Events.Unsubscribe (element);
    }

    protected override Collections.ISet<IHandler> GetConcreteSet () => Handlers;

    public override bool IsConsumable (object element)
      => element is IPipeline || element is IPipelineHandler;
  }
}