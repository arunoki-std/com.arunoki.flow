using System;
using System.Collections.Generic;
using Arunoki.Flow.Builders;
using NUnit.Framework;

namespace Arunoki.Flow.Tests
{
    public class FlowHubTests
    {
        [Test]
        public void Ctor_CollectsContainersFromHubAndRootContext()
        {
            var context = new ContainerHubContext();

            var hub = new TestableFlowHub(context);

            var containers = hub.GetContainers();
            Assert.That(containers.Count, Is.EqualTo(6));
            Assert.That(containers, Contains.Item(hub.Handlers));
            Assert.That(containers, Contains.Item(hub.Pipeline));
            Assert.That(containers, Contains.Item(hub.Contexts));
            Assert.That(containers, Contains.Item(hub.Services));
            Assert.That(containers, Contains.Item(hub.Updater));
            Assert.That(containers, Contains.Item(context.Custom));
        }

        [Test]
        public void Ctor_DummyRootContextContributesNoContainers()
        {
            var hub = new TestableFlowHub(new DummyHubContext());

            // Events is an EventBus, not an IHubContainer — plain FlowHub owns 5 built-ins.
            var containers = hub.GetContainers();
            Assert.That(containers.Count, Is.EqualTo(5));
            Assert.That(containers, Is.All.Not.Null);
        }

        [Test]
        public void SortContainers_PinsBuildOrderOfSixBuiltInsAndCustomLast()
        {
            // Built-ins use BuildOrder = short.MinValue + n while Any = 0, so a custom
            // container (and UpdatableContainer, which keeps the Any default) sorts LAST.
            var hub = new HubWithManagers(new ContainerHubContext());

            var types = new List<Type>();
            foreach (var container in hub.GetContainers())
                types.Add(container.GetType());

            // Updater (Any = 0) before Custom (Any = 0): List.Sort is an insertion sort at
            // this size (n <= 16) and therefore stable; do not rely on it for larger counts.
            Assert.That(
                types,
                Is.EqualTo(
                    new[]
                    {
                        typeof(HandlersContainer),
                        typeof(PipelineContainer),
                        typeof(ContextsContainer),
                        typeof(ManagersContainer),
                        typeof(ServicesContainer),
                        typeof(UpdatableContainer),
                        typeof(RecordingHubContainer),
                    }
                )
            );
        }

        [Test]
        public void TryInjectDependencies_FillsHubAndRootContextOnlyWhenNull()
        {
            var root = new PlainHubContext();
            var hub = new TestableFlowHub(root);
            var fresh = new InjectablePart();

            hub.InjectDependencies(fresh);

            Assert.That(fresh.HubValue, Is.SameAs(hub));
            Assert.That(fresh.ContextValue, Is.SameAs(root));

            var otherHub = new TestableFlowHub(new PlainHubContext());
            var otherContext = new PlainHubContext();
            var preset = new InjectablePart { HubValue = otherHub, ContextValue = otherContext };

            hub.InjectDependencies(preset);

            Assert.That(preset.HubValue, Is.SameAs(otherHub));
            Assert.That(preset.ContextValue, Is.SameAs(otherContext));
        }

        [Test]
        public void Register_ReturnsTrueOnlyWhenAContainerConsumesTheEntity()
        {
            var hub = new TestableFlowHub(new PlainHubContext());
            var service = new PlainHubService();

            Assert.That(hub.IsConsumable(service), Is.True);
            Assert.That(hub.Register(service), Is.True);
            Assert.That(hub.Register(service), Is.False); // duplicate is rejected
            Assert.That(CollectServices(hub), Contains.Item(service));

            var alien = new object();
            Assert.That(hub.IsConsumable(alien), Is.False);
            Assert.That(hub.Register(alien), Is.False);
        }

        [Test]
        public void Register_ForwardsToEveryConsumingContainerAndRemoveMirrorsIt()
        {
            var context = new ContainerHubContext();
            context.Custom.ConsumablePredicate = entity => entity is PlainHubService;
            var hub = new TestableFlowHub(context);
            var service = new PlainHubService();

            hub.Register(service);

            Assert.That(CollectServices(hub), Contains.Item(service));
            Assert.That(context.Custom.Registered, Is.EqualTo(new object[] { service }));

            hub.Remove(service);

            Assert.That(CollectServices(hub), Has.No.Member(service));
            Assert.That(context.Custom.Removed, Is.EqualTo(new object[] { service }));
        }

        [Test]
        public void RegisterRemoveIsConsumable_NullThrowsArgumentNullException()
        {
            var hub = new TestableFlowHub(new PlainHubContext());

            Assert.That(() => hub.Register(null), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => hub.Remove(null), Throws.TypeOf<ArgumentNullException>());
            Assert.That(() => hub.IsConsumable(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void RemoveAll_ClearsContainersAndCallsEventsClearAll()
        {
            var context = new EventfulHubContext();
            var hub = new TestableFlowHub(context);
            var service = new PlainHubService();
            hub.Register(service);
            hub.Activate(); // OnInit registers the root context => Domain channel enters Events
            var handler = new RecordingHandler("hub", null);
            hub.Events.Subscribe(handler);
            Assert.That(context.Domain.CallbackCount, Is.GreaterThan(0));

            hub.RemoveAll();

            // Events.ClearAll(): removing channels drops their handler callbacks too.
            Assert.That(context.Domain.CallbackCount, Is.Zero);
            Assert.That(CollectServices(hub), Is.Empty);
            Assert.That(CollectContexts(hub), Is.Empty);
        }

        [Test]
        public void Reset_CallsEventsResetReloadingAutoResetChannels()
        {
            var context = new EventfulHubContext();
            var hub = new TestableFlowHub(context);
            hub.Activate();
            context.AutoTrigger.Fire();
            Assert.That(context.AutoTrigger.IsTriggered, Is.True);

            hub.Reset();

            Assert.That(context.AutoTrigger.IsTriggered, Is.False);
        }

        [Test]
        public void TryFind_FindsBuiltInContainerByTypeAndFailsForAbsentType()
        {
            var hub = new TestableFlowHub(new PlainHubContext());

            Assert.That(hub.TryFind<HandlersContainer>(out var handlers), Is.True);
            Assert.That(handlers, Is.SameAs(hub.Handlers));
            Assert.That(hub.TryFind<UpdatableContainer>(out var updater), Is.True);
            Assert.That(updater, Is.SameAs(hub.Updater));

            // Plain FlowHub has no ManagersContainer (GlobalHub adds it).
            Assert.That(hub.TryFind<ManagersContainer>(out var managers), Is.False);
            Assert.That(managers, Is.Null);
        }

        [Test]
        public void ActivateDeactivate_PropagateToContainersAndOnInitAddsRootContext()
        {
            var context = new EventfulHubContext();
            var hub = new TestableFlowHub(context);
            Assert.That(hub.IsActive(), Is.False);
            Assert.That(CollectContexts(hub), Is.Empty);

            hub.Activate();

            Assert.That(hub.IsActive(), Is.True);
            Assert.That(hub.Handlers.IsActive(), Is.True);
            Assert.That(hub.Services.IsActive(), Is.True);
            Assert.That(CollectContexts(hub), Is.EqualTo(new IFlowContext[] { context }));

            hub.Deactivate();

            Assert.That(hub.IsActive(), Is.False);
            Assert.That(hub.Handlers.IsActive(), Is.False);
            Assert.That(hub.Services.IsActive(), Is.False);
        }

        private static List<IService> CollectServices(FlowHub hub)
        {
            var result = new List<IService>();
            foreach (var service in hub.Services)
                result.Add(service);
            return result;
        }

        private static List<IFlowContext> CollectContexts(FlowHub hub)
        {
            var result = new List<IFlowContext>();
            foreach (var context in hub.Contexts)
                result.Add(context);
            return result;
        }
    }
}
