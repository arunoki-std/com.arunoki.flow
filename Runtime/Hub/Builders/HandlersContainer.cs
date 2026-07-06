using Arunoki.Flow.Basics;
using Arunoki.Flow.Events.Core;

namespace Arunoki.Flow.Builders
{
    public class HandlersContainer : HubContainer<IFlowHandler>
    {
        private SubscriptionService subscriber;

        /// Encapsulates Events (Subscribe / Unsubscribe) without Handlers allocation when Hub (Activated / Deactivated).
        internal SubscriptionService Subscriber =>
            (subscriber ??= new SubscriptionService(Hub.Events));

        protected override void OnElementAdded(IFlowHandler handler)
        {
            base.OnElementAdded(handler);

            Subscriber.Register(handler);
        }

        protected override void OnElementRemoved(IFlowHandler handler)
        {
            base.OnElementRemoved(handler);

            Subscriber.Remove(handler);
        }

        protected override void OnInit()
        {
            base.OnInit();

            var list = GetAllElements();
            for (var index = 0; index < list.Count; index++)
                if (list[index] is IInitializable initializable && !initializable.IsInitialized())
                    initializable.Initialize();
        }

        public override void Reset()
        {
            base.Reset();

            var list = GetAllElements();
            for (var index = 0; index < list.Count; index++)
                if (list[index] is IFlowResettableHandler handler)
                    handler.OnReset();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Subscriber.Activate();

            var list = GetAllElements();
            for (var index = 0; index < list.Count; index++)
                if (list[index] is IFlowServiceHandler handler)
                    handler.OnActivated();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            Subscriber.Deactivate();

            var list = GetAllElements();
            for (var index = 0; index < list.Count; index++)
                if (list[index] is IFlowServiceHandler handler)
                    handler.OnDeactivated();
        }

        protected override bool IsMultiInstancesSupported() => false;

        public override int GetBuildOrder() => (int)FlowHub.BuildOrder.Handlers;
    }
}
