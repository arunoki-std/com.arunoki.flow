using Arunoki.Collections;
using Arunoki.Collections.Utilities;

using System;

namespace Arunoki.Flow.Misc
{
  public class PipelineSet : HandlerSet
  {
    protected readonly SetsTypeCollection<IHandler> TypeSet;

    public PipelineSet ()
    {
      TypeSet = new(this);
    }

    protected override ISet<IHandler> GetConcreteSet () => TypeSet;

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

    public virtual void Add<TPipeline> () where TPipeline : IPipeline
    {
      Add (typeof(TPipeline), Context);
    }

    public virtual void Add (IPipeline pipeline)
    {
      Add (pipeline.GetType (), pipeline is IContext context ? context : Context);
    }

    public virtual void Add (IPipelineHandler handler, IContext context = null)
    {
      context ??= Context;
      if (handler is IContextPart contextPart) contextPart.Set (context);
      TypeSet.GetOrCreate (context.GetType ()).Add (handler);
    }

    protected virtual void Add (Type pipelineType, IContext context = null)
    {
      //TODO: Create pipeline from scratch
      context ??= Context;
      var typeList = pipelineType.GetNestedTypes<IPipelineHandler> ();
      var set = TypeSet.GetOrCreate (pipelineType);

      for (var i = 0; i < typeList.Count; i++)
      {
        try
        {
          var handler = (IPipelineHandler) Activator.CreateInstance (typeList [i]);
          if (set is IContextPart contextPart) contextPart.Set (context);
          set.Add (handler);
        }
        catch (MissingMethodException)
        {
          throw new MissingConstructorException (typeList [i].Name);
        }
      }
    }

    public void Remove<TPipeline> () where TPipeline : IPipelineHandler
    {
      TypeSet.Clear (typeof(TPipeline));
    }

    public void Remove (IPipelineHandler handler)
    {
      TypeSet.Remove (handler);
    }

    public override void Produce (object element)
    {
      if (element is IPipeline pipeline)
      {
        Add (pipeline.GetType (), pipeline is IContext ctx ? ctx : Context);
      }
      else if (element is IPipelineHandler handler)
      {
        Add (handler);
      }
    }

    public override bool IsConsumable (object element)
      => element is IPipeline || element is IPipelineHandler;
  }
}