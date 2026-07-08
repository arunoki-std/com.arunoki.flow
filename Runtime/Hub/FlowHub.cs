using System.Collections.Generic;
using Arunoki.Flow.Basics;
using Arunoki.Flow.Builders;
using Arunoki.Flow.Collections.Utilities;
using Arunoki.Flow.Events;

namespace Arunoki.Flow
{
    public partial class FlowHub : BaseServiceExplicit
    {
        internal enum BuildOrder
        {
            Any = 0,
            Handlers = short.MinValue + 1,
            Pipelines = short.MinValue + 2,
            Contexts = short.MinValue + 3,
            Managers = short.MinValue + 4,
            Services = short.MinValue + 5,
        }

        protected internal readonly List<IHubContainer> Containers = new(8);

        public EventBus Events { get; } = new();
        public ContextsContainer Contexts { get; }
        public ServicesContainer Services { get; } = new();
        public PipelineContainer Pipeline { get; } = new();
        public HandlersContainer Handlers { get; } = new();
        public UpdatableContainer Updater { get; } = new();

        public FlowHub(IFlowContext context)
            : this(context, true) { }

        protected FlowHub(IFlowContext context, bool initParts)
        {
            TargetService = new ServiceWithElements<IHubContainer>(Containers);
            Contexts = new ContextsContainer(context, this);

            if (initParts)
                InitParts();
        }

        protected virtual void InitParts()
        {
            FindPartsAt(this);
            FindPartsAt(Contexts.Root);
        }

        public void Activate()
        {
            (this as IService).Activate();
            (this as IStartable).Start();
        }

        public void Deactivate() => (this as IService).Deactivate();

        protected virtual void FindPartsAt(object target)
        {
            if (target is IDummy)
                return;

            var prevCount = Containers.Count;

            foreach (var container in target.FindProperties<IHubContainer>())
            {
                TryInjectDependencies(container);
                Containers.Add(container);
            }

            if (Containers.Count != prevCount)
                SortContainers();
        }

        protected internal virtual void TryInjectDependencies(object entity)
        {
            if (entity is IHubPart hubPart && hubPart.Get() == null)
                hubPart.Set(this);
            if (entity is IContextPart ctxPart && ctxPart.Get() == null)
                ctxPart.Set(Contexts.Root);
        }

        protected void SortContainers() =>
            Containers.Sort((a, b) => a.GetBuildOrder().CompareTo(b.GetBuildOrder()));
    }
}
