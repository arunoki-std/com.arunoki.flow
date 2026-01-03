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
      if (element is IContextPart contextPart) contextPart.Set (Context);
      Hub.Events.Subscribe (element);
      base.OnElementAdded (element);
    }

    protected override void OnElementRemoved (IHandler element)
    {
      base.OnElementRemoved (element);
      Hub.Events.Unsubscribe (element);
      if (element is IDisposable disposable) disposable.Dispose ();
    }

    public virtual void Add<TPipeline> () where TPipeline : IPipeline
    {
      Add (typeof(TPipeline));
    }

    public virtual void Add (IPipelineHandler handler)
    {
      TypeSet.Add (Context.GetType (), handler);
    }

    protected virtual void Add (Type pipelineType)
    {
      var types = pipelineType.GetNestedTypes<IPipelineHandler> ();
      var set = TypeSet.GetOrCreate (pipelineType);

      for (var i = 0; i < types.Count; i++)
      {
        try
        {
          var handler = (IPipelineHandler) Activator.CreateInstance (types [i]);
          set.Add (handler);
        }
        catch (MissingMethodException)
        {
          throw new MissingConstructorException (types [i].Name);
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

    public override void Build (object element)
    {
      if (element is IPipeline)
      {
        Add (element.GetType ());
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