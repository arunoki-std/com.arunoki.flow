using Arunoki.Collections.Utilities;
using Arunoki.Flow.Utilities;

using System;
using System.Collections.Generic;

namespace Arunoki.Flow
{
  public partial class FlowHub : IBuilder
  {
    public void Build (object element)
    {
      switch (element)
      {
        case IContext context:
          var contextsList = context.GetAllPropertiesWithNested<IContext> ();
          contextsList.Insert (0, context);
          BuildNestedContexts (contextsList);
          break;

        case Type staticType:
          if (Utils.IsDebug () && !IsConsumable (staticType))
            throw new StaticManagerException (staticType);

          Events.Register (staticType);
          Events.Subscribe (staticType);
          BuildNestedContexts (staticType.GetAllPropertiesWithNested<IContext> ());
          break;

        default:
          BuildBySets (element);
          break;
      }
    }

    protected virtual void BuildNestedContexts (List<IContext> contexts)
    {
      contexts.ForEach (Events.Register);
      contexts.ForEach (BuildBySets);
    }

    protected virtual void BuildBySets (object obj)
    {
      if (obj is IHandler eventsHandler) Events.Subscribe (eventsHandler);

      ForEachSet<IBuilder> (builder =>
      {
        if (builder.IsConsumable (obj)) builder.Build (obj);
      });
    }

    public bool IsConsumable (object element)
    {
      return element is IContext || ForAnySet<IBuilder> (builder => builder.IsConsumable (element));
    }

    public bool IsConsumable (Type staticType)
    {
      return staticType.IsAbstract && staticType.IsSealed;
    }
  }
}