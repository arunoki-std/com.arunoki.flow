using Arunoki.Collections;
using Arunoki.Collections.Utilities;

using System;

namespace Arunoki.Flow.Misc
{
  public class ManagersSet : Set<Type>, IBuilder
  {
    protected readonly FlowHub Hub;

    protected ManagersSet (FlowHub hub) => Hub = hub;

    public ManagersSet (FlowHub hub, Type staticType) : this (hub) => Produce (staticType);

    void IBuilder.Produce (object element)
    {
      switch (element)
      {
        case Type manager:
          Produce (manager);
          break;
      }
    }

    public void Produce (Type manager)
    {
      if (!IsTypeStatic (manager))
        throw new StaticManagerException (manager);

      Add (manager);
    }

    protected override void OnElementAdded (Type manager)
    {
      base.OnElementAdded (manager);

      // 1. add reactive properties
      Hub.Events.AddEventSource (manager);

      // 2. add contexts
      var contextList = manager.GetAllPropertiesWithNested<IContext> ();
      if (contextList.Count > 0)
      {
        Hub.AllContexts.Add (manager, contextList.ToArray ());
      }
    }

    protected override void OnElementRemoved (Type manager)
    {
      base.OnElementRemoved (manager);

      Hub.Events.RemoveEvents (manager);
      Hub.AllContexts.Clear (manager);
    }

    public bool IsConsumable (object element)
      => element is Type type && IsTypeStatic (type);

    public static bool IsTypeStatic (Type type)
      => type.IsAbstract && type.IsSealed;
  }
}