using System;
using System.Reflection;
using Arunoki.Flow.Basics;
using Arunoki.Flow.Collections.Utilities;
using Arunoki.Flow.Utilities;

namespace Arunoki.Flow.Builders
{
    public class ManagersContainer : HubContainer<Type>
    {
        public ManagersContainer(FlowHub hub)
        {
            (this as IHubPart).Set(hub);
        }

        public ManagersContainer(FlowHub hub, Type staticType)
            : this(hub)
        {
            Set.TryAdd(staticType);
        }

        protected override void OnInit()
        {
            base.OnInit();

            foreach (Type staticType in this)
            {
                SubscribeHandlers(staticType);
            }
        }

        protected override void OnElementAdded(Type staticType)
        {
            base.OnElementAdded(staticType);

            Hub.Events.RegisterSource(staticType);

            Hub.Contexts.KeySet.GetOrCreate(staticType)
                .AddRange(staticType.FindPropertiesWithNested<IFlowContext>().ToArray());

            Hub.Services.KeySet.GetOrCreate(staticType)
                .AddRange(
                    staticType
                        .FindProperties<IService>(
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                        )
                        .ToArray()
                );
        }

        protected override void OnElementRemoved(Type staticType)
        {
            base.OnElementRemoved(staticType);

            Hub.Events.UnregisterSource(staticType);
            Hub.Events.Unsubscribe(staticType);

            Hub.Services.KeySet.Clear(staticType);
            Hub.Contexts.KeySet.Clear(staticType);
        }

        private void SubscribeHandlers(Type staticType)
        {
            Hub.Handlers.Subscriber.Register(staticType);
        }

        public override bool IsConsumable(Type staticType) =>
            staticType != null && staticType.IsStatic();

        protected override bool CanBuildAfterHubInit() => false;

        protected override bool CanBuildAfterHubStarted() => false;

        protected override bool CanBuildAfterHubActivation() => false;

        protected override bool IsMultiInstancesSupported() => false;

        public override int GetBuildOrder() => (int)FlowHub.BuildOrder.Managers;
    }
}
