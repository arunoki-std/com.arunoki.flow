using Arunoki.Collections;
using Arunoki.Collections.Utilities;
using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Misc
{
  public class ManagersSet : SetsTypeCollection<IContext>, IBuilder
  {
    protected readonly FlowHub Hub;
    private bool isInitialized;

    public ManagersSet (FlowHub hub) => Hub = hub;

    public ManagersSet (FlowHub hub, Type staticType) : this (hub) => Produce (staticType);

    protected internal void TryInitialize ()
    {
      if (!isInitialized)
      {
        isInitialized = true;
        OnInitialized ();
      }
    }

    private void OnInitialized ()
    {
      foreach (var manager in SetsCache.Keys)
        Hub.Handlers.Produce (manager);
    }

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
      if (Utils.IsDebug () && !IsTypeStatic (manager))
        throw new StaticManagerException (manager);

      Add (manager, manager.GetAllPropertiesWithNested<IContext> ().ToArray ());
    }

    protected override void OnKeyAdded (Type manager)
    {
      base.OnKeyAdded (manager);

      Hub.Events.AddEventSource (manager);
    }

    protected override void OnKeyRemoved (Type manager)
    {
      base.OnKeyRemoved (manager);

      Hub.Events.RemoveEvents (manager);
    }

    protected override void OnElementAdded (IContext context)
    {
      base.OnElementAdded (context);

      Hub.Contexts.Produce (context);
    }

    protected override void OnElementRemoved (IContext element)
    {
      base.OnElementRemoved (element);

      Hub.Contexts.Remove (element);
    }

    public bool IsConsumable (object element)
      => element is Type type && IsTypeStatic (type);

    public static bool IsTypeStatic (Type type)
      => type.IsAbstract && type.IsSealed;
  }
}