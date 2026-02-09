using Arunoki.Collections;
using Arunoki.Collections.Enumerators;
using Arunoki.Collections.Utilities;
using Arunoki.Flow.Utilities;

using System;

namespace Arunoki.Flow.Basics
{
  public class ManagersBuilder : BaseHubBuilder<Type>
  {
    public Set<Type> StaticTypes { get; }

    public ManagersBuilder (FlowHub hub)
    {
      (this as IHubPart).Set (hub);
      StaticTypes = new(this, IsConsumable);
    }

    public ManagersBuilder (FlowHub hub, Type staticType)
      : this (hub)
    {
      StaticTypes.TryAdd (staticType);
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized ();

      foreach (Type staticType in StaticTypes)
        SubscribeHandlers (staticType);
    }

    public override void Produce (Type staticType)
    {
      base.Produce (staticType);

      StaticTypes.TryAdd (staticType);

      if (IsInitialized ())
      {
        if (Utils.IsWarningsEnabled ())
          UnityEngine.Debug.LogWarning (
            $"Event subscription may not be applied for '{staticType.Name}'. Produce static managers before hub activation.");

        SubscribeHandlers (staticType);
      }
    }

    protected override void OnElementAdded (Type staticType)
    {
      base.OnElementAdded (staticType);

      Hub.Events.RegisterSource (staticType);

      Hub.Contexts.Elements.AddRange (staticType.FindPropertiesWithNested<IContext> ().ToArray ());
      Hub.Services.Elements.AddRange (staticType.FindPropertiesWithNested<IService> ().ToArray ());
    }

    protected override void OnElementRemoved (Type staticType)
    {
      base.OnElementRemoved (staticType);

      Hub.Events.UnregisterSource (staticType);
      Hub.Events.Unsubscribe (staticType);

      foreach (var context in staticType.FindPropertiesWithNested<IContext> ().ToArray ())
        Hub.Contexts.Clear (context);

      foreach (var service in staticType.FindPropertiesWithNested<IService> ().ToArray ())
        Hub.Services.Clear (service);
    }

    public override void ClearAll ()
    {
      StaticTypes.Clear ();
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
    
    public override MutableEnumerator<Type> GetEnumerator () => StaticTypes.GetEnumerator ();
    public override MutableCastEnumerable<Type, T> Cast<T> () => StaticTypes.Cast<T> ();
  }
}