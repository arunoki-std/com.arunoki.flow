using Arunoki.Collections;
using Arunoki.Collections.Utilities;

using System;

namespace Arunoki.Flow.Misc
{
  public class ContextSet : SetsTypeCollection<IContext>, IBuilder
  {
    protected readonly FlowHub Hub;

    protected ContextSet (FlowHub hub) => Hub = hub;

    public ContextSet (FlowHub hub, Type staticType) : this (hub) => Produce (staticType);

    public ContextSet (FlowHub hub, IContext context) : this (hub) => Produce (context);

    void IBuilder.Produce (object element)
    {
      switch (element)
      {
        case IContext ctx:
          Produce (ctx);
          break;

        case Type manager:
          Produce (manager);
          break;
      }
    }

    public void Produce (IContext context)
    {
      var contextType = context.GetType ();

      Add (contextType, context);
      Add (contextType, context.GetAllPropertiesWithNested<IContext> ().ToArray ());
    }

    public void Produce (Type staticManager)
    {
      if (!IsTypeStatic (staticManager))
        throw new StaticManagerException (staticManager);

      Add (staticManager, staticManager.GetAllPropertiesWithNested<IContext> ().ToArray ());
    }

    protected override void OnElementAdded (IContext context)
    {
      base.OnElementAdded (context);

      Hub.Events.AddEventSource (context);
    }

    protected override void OnElementRemoved (IContext context)
    {
      base.OnElementRemoved (context);

      Hub.Events.RemoveEvents (context);
    }

    protected override void OnKeyAdded (Type keyType)
    {
      base.OnKeyAdded (keyType);

      if (IsTypeStatic (keyType))
        Hub.Events.AddEventSource (keyType);
    }

    protected override void OnKeyRemoved (Type keyType)
    {
      base.OnKeyRemoved (keyType);

      if (IsTypeStatic (keyType))
        Hub.Events.RemoveEvents (keyType);
    }

    public bool IsConsumable (object element)
      => element is IContext || (element is Type type && IsTypeStatic (type));

    public static bool IsTypeStatic (Type type)
      => type.IsAbstract && type.IsSealed;
  }
}