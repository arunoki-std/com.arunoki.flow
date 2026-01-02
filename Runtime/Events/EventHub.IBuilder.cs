using Arunoki.Collections.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class EventHub : IBuilder
  {
    public void Build (object item)
    {
      switch (item)
      {
        case IEventsContext context:
          var contextsList = context.GetAllPropertiesWithNested<IEventsContext> ();
          contextsList.Insert (0, context);
          BuildNestedContexts (contextsList);
          break;

        case Type staticType:
          if (Globals.IsDebug () && !IsConsumable (staticType))
            throw new StaticManagerException (staticType);

          Events.Register (staticType);
          Events.Subscribe (staticType);
          BuildNestedContexts (staticType.GetAllPropertiesWithNested<IEventsContext> ());
          break;

        default:
          BuildBySets (item);
          break;
      }
    }

    protected virtual void BuildNestedContexts (List<IEventsContext> contexts)
    {
      contexts.ForEach (Events.Register);
      contexts.ForEach (BuildBySets);
    }

    protected virtual void BuildBySets (object obj)
    {
      if (obj is IEventsHandler eventsHandler) Events.Subscribe (eventsHandler);

      ForEachSet<IBuilder> (builder =>
      {
        if (builder.IsConsumable (obj)) builder.Build (obj);
      });
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