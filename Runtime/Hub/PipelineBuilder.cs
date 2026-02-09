using Arunoki.Collections;
using Arunoki.Collections.Utilities;

using System;

namespace Arunoki.Flow.Basics
{
  public class PipelineBuilder : BaseHubCollection<IPipeline>
  {
    public PipelineBuilder (IContainer<IPipeline> rootContainer = null) : base (rootContainer)
    {
    }

    protected virtual HandlersBuilder Handlers => Hub.Handlers;

    public void Produce<TPipeline> () where TPipeline : IPipeline, new ()
    {
      Produce (Activator.CreateInstance (typeof(TPipeline)) as IPipeline);
    }

    public bool Clear<TPipeline> () where TPipeline : IPipeline
    {
      return Clear (typeof(TPipeline));
    }

    public bool Clear (Type pipelineType)
    {
      foreach ((int index, IPipeline element) in WithIndex ())
      {
        if (element.GetType () == pipelineType)
        {
          Elements.RemoveAt (index);
          return true;
        }
      }

      return false;
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
  }
}