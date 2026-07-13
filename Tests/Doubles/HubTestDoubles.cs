using System;
using System.Collections.Generic;
using Arunoki.Flow.Basics;
using Arunoki.Flow.Builders;

namespace Arunoki.Flow.Tests
{
    /// Exposes the protected internal surface of <see cref="FlowHub"/> for tests
    /// (RF-004 access strategy: test subclasses instead of InternalsVisibleTo).
    internal class TestableFlowHub : FlowHub
    {
        public TestableFlowHub(IFlowContext context)
            : base(context) { }

        protected TestableFlowHub(IFlowContext context, bool initParts)
            : base(context, initParts) { }

        public IReadOnlyList<IHubContainer> GetContainers() => Containers;

        public void InjectDependencies(object entity) => TryInjectDependencies(entity);
    }

    /// Replicates GlobalHub's shape (a public ManagersContainer property created before
    /// InitParts) without touching GlobalHub statics — lets FlowHubTests pin the build
    /// order of all six built-in containers in plain CLR.
    internal sealed class HubWithManagers : TestableFlowHub
    {
        public HubWithManagers(IFlowContext context)
            : base(context, false)
        {
            Managers = new ManagersContainer(this);
            InitParts();
        }

        public ManagersContainer Managers { get; }
    }

    internal sealed class PlainHubContext : IFlowContext { }

    internal sealed class DummyHubContext : IFlowContext, IDummy { }

    /// Root context with event channels — for observing Events.ClearAll()/Reset() via FlowHub.
    internal sealed class EventfulHubContext : IFlowContext
    {
        public TestSignal<TestDomainEvent> Domain { get; } = new();
        public TestTrigger<AutoTriggerEvent> AutoTrigger { get; } = new(autoReset: true);
    }

    /// Root context exposing a custom IHubContainer property — for FindPartsAt collection
    /// and the custom-containers-sort-last pin.
    internal sealed class ContainerHubContext : IFlowContext
    {
        public RecordingHubContainer Custom { get; } = new();
    }

    /// Minimal custom container implementing IHubContainer directly.
    /// GetBuildOrder() == 0 == FlowHub.BuildOrder.Any (the default for custom containers).
    internal sealed class RecordingHubContainer : IHubContainer
    {
        public readonly List<object> Registered = new();
        public readonly List<object> Removed = new();
        public int RemoveAllCalls;
        public Func<object, bool> ConsumablePredicate = _ => false;

        public bool Register(object element)
        {
            Registered.Add(element);
            return true;
        }

        public void Remove(object element) => Removed.Add(element);

        public void RemoveAll() => RemoveAllCalls++;

        public bool IsConsumable(object element) => ConsumablePredicate(element);

        public int GetBuildOrder() => 0;
    }

    /// Plain part with observable backing fields; Set() has no rewrite guard so tests
    /// control the pre-set state directly.
    internal sealed class InjectablePart : IHubPart, IContextPart
    {
        public FlowHub HubValue;
        public IFlowContext ContextValue;

        FlowHub IHubPart.Get() => HubValue;

        void IHubPart.Set(FlowHub value) => HubValue = value;

        IFlowContext IContextPart.Get() => ContextValue;

        void IContextPart.Set(IFlowContext value) => ContextValue = value;
    }

    /// Plain service — consumable by ServicesContainer only (not IUpdatable, not a context).
    internal sealed class PlainHubService : BaseService { }

    /// Static class with no channel properties — safe to register into Managers repeatedly
    /// across tests (RegisterSource finds nothing, so no channel-context rewrite throws).
    internal static class HubManagedStatic { }
}
