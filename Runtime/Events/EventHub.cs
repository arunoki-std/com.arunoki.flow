using Arunoki.Flow.Collections;
using Arunoki.Flow.Misc;
using Arunoki.Flow.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Arunoki.Flow
{
  public class EventHub : GroupHolder<IEventReceiver>, IBuilder
  {
    public IEventsContext Context { get; private set; }

    public EventBus Events { get; } = new();

    public ControllersGroup Controllers { get; } = new();

    public void Init (IEventsContext eventsContext)
    {
      Context = eventsContext;

      TrySetTargetHandler (Context);

      AddGroupsFrom (this);
      AddGroupsFrom (Context);

      ForEachGroup<IGroupHandler<IEventReceiver>> (handler => handler.TargetGroupHandler = this);
      ForEachGroup<IEventsHubPart> (part => part.Init (this));

      Build (Context);
    }

    public void Build (object item)
    {
      switch (item)
      {
        case IEventsContext context:
          var list = context.GetAllPropertiesWithNested<IEventsContext> ();
          list.Insert (0, context);
          BuildObject (list);
          break;

        case Type staticType:
          if (Globals.IsDebug () && !IsConsumable (staticType))
            throw new Exception ($"{staticType.Name} is not consumable. Type must be static.");

          Events.Register (staticType);
          Events.Subscribe (staticType);
          BuildObject (staticType.GetAllPropertiesWithNested<IEventsContext> ());
          break;

        default:
          BuildContexts (item);
          break;
      }
    }

    protected virtual void BuildObject (List<IEventsContext> contexts)
    {
      contexts.ForEach (Events.Register);
      contexts.ForEach (BuildContexts);
    }

    protected virtual void BuildContexts (object obj)
    {
      ForEachGroup<IBuilder> (builder =>
      {
        if (builder.IsConsumable (obj))
          builder.Build (obj);
      });

      if (obj is IEventReceiver receiver)
        Events.Subscribe (receiver);
    }

    public override void Dispose ()
    {
      base.Dispose ();

      Events.Dispose ();
    }

    public bool IsConsumable (object item)
    {
      return item is IEventsContext || ForEachGroup<IBuilder> ().Any (builder => builder.IsConsumable (item));
    }

    public bool IsConsumable (Type staticType)
    {
      return staticType.IsAbstract && staticType.IsSealed;
    }
  }
}