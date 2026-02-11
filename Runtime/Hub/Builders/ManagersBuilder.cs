using Arunoki.Collections.Utilities;
using Arunoki.Flow.Basics;
using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Builders
{
  public class ManagersBuilder : HubBuilder<Type>
  {
    public ManagersBuilder (FlowHub hub)
    {
      (this as IHubPart).Set (hub);
    }

    public ManagersBuilder (FlowHub hub, Type staticType)
      : this (hub)
    {
      Set.TryAdd (staticType);
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      foreach (Type staticType in this)
        SubscribeHandlers (staticType);
    }

    protected override void OnElementAdded (Type staticType)
    {
      base.OnElementAdded (staticType);

      Hub.Events.RegisterSource (staticType);

      Hub.Contexts.KeySet.GetOrCreate (staticType)
        .AddRange (staticType.FindPropertiesWithNested<IContext> ().ToArray ());

      Hub.Services.KeySet.GetOrCreate (staticType)
        .AddRange (staticType.FindProperties<IService> ().ToArray ());
    }

    protected override void OnElementRemoved (Type staticType)
    {
      base.OnElementRemoved (staticType);

      Hub.Events.UnregisterSource (staticType);
      Hub.Events.Unsubscribe (staticType);

      Hub.Services.KeySet.Clear (staticType);
      Hub.Contexts.KeySet.Clear (staticType);
    }

    private void SubscribeHandlers (Type staticType)
    {
      Hub.Handlers.Subscriber.Register (staticType);
    }

    public override bool IsConsumable (Type staticType)
      => staticType != null && staticType.IsStatic ();

    protected override bool CanBuildAfterHubInit () => false;
    protected override bool CanBuildAfterHubStarted () => false;
    protected override bool CanBuildAfterHubActivation () => false;
    protected override bool IsMultiInstancesSupported () => false;

    protected override bool IsCompositionInitializable () => false;
    protected override bool IsCompositionResettable () => false;
    protected override bool IsCompositionStartable () => false;
    protected override bool IsCompositionServiceAvailable () => false;

    protected internal override int GetBuildOrder () => (int) FlowHub.BuildOrder.Managers;
  }
}