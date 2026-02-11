using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;

using System;

namespace Arunoki.Flow.Builders
{
  public class PipelineBuilder : HubBuilder<IPipeline>
  {
    protected virtual HandlersBuilder Handlers => Hub.Handlers;

    public void Produce<TPipeline> () where TPipeline : IPipeline, new ()
    {
      Produce (Activator.CreateInstance (typeof(TPipeline)) as IPipeline);
    }

    public void Clear<TPipeline> () where TPipeline : IPipeline
    {
      Clear (typeof(TPipeline));
    }

    public void Clear (Type pipelineType)
    {
      foreach (IPipeline pipeline in this)
      {
        if (pipeline.GetType () == pipelineType)
        {
          Clear (pipeline);
          break;
        }
      }
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

    protected override bool IsMultiInstancesSupported () => false;
    protected internal override int GetBuildOrder () => (int) FlowHub.BuildOrder.Pipelines;
  }
}