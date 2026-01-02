using Arunoki.Collections;
using Arunoki.Collections.Utilities;

using System;

namespace Arunoki.Flow.Misc
{
  public class ControllersGroup : SetsTypeCollection<IEventsHandler>, IEventsHubPart, IBuilder
  {
    public IEventsContext Context { get; private set; }
    IEventsContext IEventsContextPart.Context { get => Context; set { Context = value; } }
    public EventHub Hub { get; private set; }

    public virtual void Init (EventHub hub)
    {
      Hub = hub;
      Context = hub.Context;
    }

    protected override void OnElementAdded (IEventsHandler element)
    {
      if (element is IEventsContextPart contextPart) contextPart.Context = Context;
      Hub.Events.Subscribe (element);
      base.OnElementAdded (element);
    }

    protected override void OnElementRemoved (IEventsHandler element)
    {
      base.OnElementRemoved (element);
      Hub.Events.Unsubscribe (element);
      if (element is IDisposable disposable) disposable.Dispose ();
    }

    public virtual void Add<T> () where T : IControllersContainer
    {
      Add (typeof(T));
    }

    public virtual void Add (IController controller)
    {
      Add (Context.GetType (), controller);
    }

    protected virtual void Add (Type containerType)
    {
      var types = containerType.GetNestedTypes<IController> ();
      var set = GetOrCreate (containerType);

      for (var i = 0; i < types.Count; i++)
      {
        try
        {
          var controller = (IController) Activator.CreateInstance (types [i]);
          set.Add (controller);
        }
        catch (MissingMethodException)
        {
          throw new MissingConstructorException (types [i].Name);
        }
      }
    }

    public void Remove<T> () where T : IControllersContainer
    {
      Clear (typeof(T));
    }

    public void Remove (IController controller)
    {
      Remove (controller as IEventsHandler);
    }

    public virtual void Build (object item)
    {
      if (item is IControllersContainer)
      {
        Add (item.GetType ());
      }
      else if (item is IController controller)
      {
        Add (controller);
      }
    }

    public virtual bool IsConsumable (object item)
      => item is IController || item is IControllersContainer;
  }
}