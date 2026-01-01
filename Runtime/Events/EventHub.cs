using Arunoki.Collections;
using Arunoki.Collections.Utils;
using Arunoki.Flow.Misc;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public class EventHub : GroupSet<IEventsHandler>, IBuilder
  {
    public IEventsContext Context { get; private set; }

    public EventBus Events { get; } = new();

    public ControllersGroup Controller { get; } = new();

    public void Init (IEventsContext eventsContext)
    {
      Context = eventsContext;

      TrySetTargetHandler (Context);

      FindSetsAt (this);
      FindSetsAt (Context);

      ForEachSet<ISetHandler<IEventsHandler>> (handler => handler.TargetSetHandler = this);
      ForEachSet<IEventsHubPart> (part => part.Init (this));

      Build (Context);
    }

    public void Build (object item)
    {
      switch (item)
      {
        case IEventsContext context:
          var contextsList = context.GetAllPropertiesWithNested<IEventsContext> ();
          contextsList.Insert (0, context);
          Build (contextsList);
          break;

        case Type staticType:
          if (Globals.IsDebug () && !IsConsumable (staticType))
            throw new Exception ($"{staticType.Name} is not consumable. Type must be static.");

          Events.Register (staticType);
          Events.Subscribe (staticType);
          Build (staticType.GetAllPropertiesWithNested<IEventsContext> ());
          break;

        default:
          BuildObject (item);
          break;
      }
    }

    protected virtual void Build (List<IEventsContext> contexts)
    {
      contexts.ForEach (Events.Register);
      contexts.ForEach (BuildObject);
    }

    protected virtual void BuildObject (object obj)
    {
      if (obj is IEventsHandler receiver)
        Events.Subscribe (receiver);

      ForEachSet<IBuilder> (builder =>
      {
        if (builder.IsConsumable (obj)) builder.Build (obj);
      });
    }

    public override void Clear ()
    {
      base.Clear ();

      Events.Dispose ();
    }

    public bool IsConsumable (object item)
    {
      return item is IEventsContext || ForAnySet<IBuilder> (builder => builder.IsConsumable (item));
    }

    public bool IsConsumable (Type staticType)
    {
      return staticType.IsAbstract && staticType.IsSealed;
    }
  }
}