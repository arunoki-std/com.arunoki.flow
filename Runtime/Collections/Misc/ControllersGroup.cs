using Arunoki.Flow.Collections;
using Arunoki.Flow.Utils;

using System;
using System.Linq;

namespace Arunoki.Flow.Misc
{
  public class ControllersGroup : KeyTypeGroups<IEventReceiver>, IEventsHubPart, IBuilder
  {
    private static readonly Type BaseControllerType = typeof(IController);

    public IEventsContext Context => Hub.Context;

    public EventHub Hub { get; private set; }

    public virtual void Init (EventHub hub)
    {
      Hub = hub;
    }

    protected override void OnAdded (IEventReceiver element)
    {
      base.OnAdded (element);

      Hub.Events.Subscribe (element);
    }

    protected override void OnRemoved (IEventReceiver element)
    {
      base.OnRemoved (element);

      Hub.Events.Unsubscribe (element);

      if (element is IDisposable disposable)
        disposable.Dispose ();
    }

    public virtual void Add<T> (IEventsContext context = null) where T : IControllersContainer
    {
      Add (typeof(T), context ?? Context);
    }

    public virtual void Add (IController controller, IEventsContext context = null)
    {
      context ??= Context;
      Add (context.GetType (), controller);
    }

    protected virtual void Add (Type containerType, IEventsContext eventsContext)
    {
      try
      {
        Add (containerType,
          containerType
            .GetNestedTypes<IController> ()
            .Select (receiverType => (IEventReceiver) Activator.CreateInstance (receiverType, eventsContext))
            .ToArray ()
        );
      }
      catch (MissingMethodException e)
      {
        UnityEngine.Debug.LogError (e);
      }
    }

    public void Remove<T> () where T : IControllersContainer
    {
      Remove (typeof(T));
    }

    public void Remove (IController controller)
    {
      Remove (controller as IEventReceiver);
    }

    public virtual void Build (object item)
    {
      if (item is IControllersContainer)
      {
        var context = item is IEventsContext c ? c : item is IEventsContextPart p ? p.Context : Context;
        Add (item.GetType (), context);
      }
      else if (item is IController controller)
      {
        Add (controller, controller is IEventsContextPart p ? p.Context : Context);
      }
    }

    public virtual bool IsConsumable (Type itemType)
      => (itemType == BaseControllerType || BaseControllerType.IsAssignableFrom (itemType) && !itemType.IsAbstract);

    public virtual bool IsConsumable (object item)
      => item is IController || item is IControllersContainer;
  }
}