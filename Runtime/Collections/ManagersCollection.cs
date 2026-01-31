using Arunoki.Collections;
using Arunoki.Collections.Utilities;
using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Collections
{
  public class ManagersCollection : SetsTypeCollection<IContext>
  {
    protected readonly FlowHub Hub;
    private bool isInitialized;

    public ManagersCollection (FlowHub hub) => Hub = hub;

    public ManagersCollection (FlowHub hub, Type staticType) : this (hub) => Add (staticType);

    protected internal void Initialize ()
    {
      if (!isInitialized)
      {
        isInitialized = true;
        OnInitialized ();
      }
    }

    protected virtual void OnInitialized ()
    {
      foreach (var manager in SetsCache.Keys)
        Hub.Handlers.GetSubscriber ().Subscribe (manager);
    }

    public void Add (Type manager)
    {
      if (Utils.IsDebug () && !manager.IsStatic ())
        throw new StaticManagerException (manager);

      Add (manager, manager.GetAllPropertiesWithNested<IContext> ().ToArray ());
    }

    protected override void OnKeyAdded (Type manager)
    {
      base.OnKeyAdded (manager);

      Hub.Events.RegisterSource (manager);
    }

    protected override void OnKeyRemoved (Type manager)
    {
      base.OnKeyRemoved (manager);

      Hub.Events.UnregisterSource (manager);
    }

    protected override void OnElementAdded (IContext context)
    {
      base.OnElementAdded (context);

      Hub.Contexts.Add (context);
    }

    protected override void OnElementRemoved (IContext element)
    {
      base.OnElementRemoved (element);

      Hub.Contexts.Remove (element);
    }
  }
}